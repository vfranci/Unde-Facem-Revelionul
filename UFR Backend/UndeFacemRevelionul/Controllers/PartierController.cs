﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UndeFacemRevelionul.ContextModels;
using UndeFacemRevelionul.Models;
using UndeFacemRevelionul.ViewModels;

namespace UndeFacemRevelionul.Controllers;

public class PartierController : Controller
{
    private readonly RevelionContext _context;
    private readonly ILogger<AccountController> _logger;

    public PartierController(RevelionContext context, ILogger<AccountController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        var currentUserId = GetCurrentUserId();

        // Găsim partier-ul asociat utilizatorului logat
        var partier = _context.Partiers.FirstOrDefault(p => p.UserId == currentUserId);

        if (partier == null)
        {
            TempData["ErrorMessage"] = "Nu există un partier asociat utilizatorului logat.";
            return RedirectToAction("Index", "Home");
        }

        // Găsim toate petrecerile asociate utilizatorului
        var parties = _context.PartyPartiers
            .Where(pp => pp.PartierId == partier.Id)
            .Select(pp => pp.Party)
            .ToList();

        // Găsim rangul utilizatorului logat pe baza punctelor
        var rank = _context.Ranks
            .FirstOrDefault(r => partier.Points >= r.MinPoints && partier.Points <= r.MaxPoints);

        // Transmitem datele către View prin ViewBag
        ViewBag.Parties = parties;
        ViewBag.RankName = rank?.Name ?? "N/A";
        ViewBag.PartierPoints = partier.Points;

        return View();
    }


    // GET: CreateParty
    [HttpGet]
    public IActionResult CreateParty()
    {
        return View(new CreatePartyViewModel());
    }

    // POST: CreateParty
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateParty(CreatePartyViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Creăm petrecerea folosind datele din CreatePartyViewModel
            var party = new PartyModel
            {
                Name = model.Name,
                TotalBudget = model.TotalBudget,
                RemainingBudget = model.TotalBudget, // Bugetul rămas este același ca bugetul total la început
                Date = model.Date,
                TotalPoints = 0, // Punctele sunt 0 inițial
                LocationId = model.LocationId, // Locația este opțională, se setează dacă există
                FoodMenuId = model.FoodMenuId, // Meniul este opțional, se setează dacă există
                PartyUsers = new List<PartyPartierModel>() // Inițializăm lista PartyUsers
            };

            // Adăugăm petrecerea în baza de date
            _context.Parties.Add(party);
            _context.SaveChanges(); // Salvează pentru a genera ID-ul petrecerii

            var currentUserId = GetCurrentUserId();

            // 2. Căutăm în baza de date `Partier`-ul care are `UserId` egal cu `currentUserId`
            var partier = _context.Partiers.FirstOrDefault(p => p.UserId == currentUserId);

            if (partier == null)
            {
                // Dacă nu găsim un `Partier` cu acest `UserId`, ar trebui să tratezi această situație.
                _logger.LogError($"Partier with UserId {currentUserId} not found.");
                return RedirectToAction("Error", "Home"); // Sau alte acțiuni, în funcție de logică
            }

            // 3. După ce am găsit `Partier`-ul, folosim ID-ul acestuia pentru a crea un `PartyPartierModel`
            var partyUser = new PartyPartierModel
            {
                PartyId = party.Id, // ID-ul petrecerii
                PartierId = partier.Id // ID-ul `Partier`-ului găsit din baza de date
            };

            // Adăugăm utilizatorul curent în lista PartyUsers
            party.PartyUsers.Add(partyUser);




            // Salvează relația între petrecere și utilizatorul creator
            _context.PartyPartiers.Add(partyUser);

            // Salvează totul într-o singură operațiune
            _context.SaveChanges(); // Salvează petrecerea și relația

            UpdatePartyTotalPoints(party.Id);

            // Redirecționare după succes
            return RedirectToAction("Dashboard", "Partier");
        }

        // Dacă modelul nu este valid, loghează erorile
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError($"Error in field: {error.ErrorMessage}");
            }
        }



        // Reîncarcă formularul dacă există erori
        return View(model);
    }

    private int GetCurrentUserId()
    {
        // Verifică dacă utilizatorul este autentificat
        if (!User.Identity.IsAuthenticated)
        {
            _logger.LogError("User is not authenticated.");
            throw new InvalidOperationException("User is not authenticated.");
        }

        // Adaugă log pentru a verifica ce claim-uri sunt disponibile
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        foreach (var claim in claims)
        {
            _logger.LogInformation($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }

        // Căutăm Claim-ul pentru NameIdentifier
        var userIdClaim = User.FindFirst("UserId");


        if (userIdClaim == null)
        {
            _logger.LogError("User does not have a valid ID claim.");
            throw new InvalidOperationException("User does not have a valid ID claim.");
        }

        // Returnează ID-ul utilizatorului
        return int.Parse(userIdClaim.Value);
    }

    public IActionResult PartyDetails(int id)
    {
        var currentUserId = GetCurrentUserId();

        var party = _context.Parties
            .Include(p => p.Tasks)
            .Include(p => p.PartyUsers)
            .ThenInclude(pu => pu.Partier)
            .FirstOrDefault(p => p.Id == id);

        if (party == null)
        {
            return NotFound();
        }

        var currentPartier = _context.Partiers.FirstOrDefault(p => p.UserId == currentUserId);

        if (currentPartier == null)
        {
            TempData["ErrorMessage"] = "Partierul nu a fost găsit.";
            return RedirectToAction("Dashboard", "Partier");
        }

        var assignedTasks = party.Tasks.Where(t => t.PartierId == currentPartier.Id).ToList();

        var tasks = party.Tasks.ToList();
        ViewBag.AssignedTasks = assignedTasks;
        ViewBag.CurrentPartier = currentPartier;
        ViewBag.AllTasks = _context.Tasks.Where(x => x.PartyId == id).ToList();

        var partyPartiers = _context.PartyPartiers
            .Where(pp => pp.PartyId == id)
            .Include(pp => pp.Partier)
            .ThenInclude(p => p.User)
            .Select(pp => pp.Partier)
            .ToList();

        var partierNames = partyPartiers.Select(partier => partier.User.Name).ToList();
        ViewBag.PartierNames = partierNames;

        UpdatePartyTotalPoints(id);

        return View(party);
    }


    // GET: EditParty
    [HttpGet]
    public IActionResult EditParty(int id)
    {
        var party = _context.Parties.FirstOrDefault(p => p.Id == id);

        if (party == null)
        {
            return NotFound();
        }

        return View(party);
    }

    // POST: EditParty
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditParty(PartyModel updatedParty)
    {
        if (!ModelState.IsValid)
        {
            return View(updatedParty);
        }

        var party = _context.Parties.FirstOrDefault(p => p.Id == updatedParty.Id);

        if (party == null)
        {
            return NotFound();
        }

        party.Name = updatedParty.Name;
        party.TotalBudget = updatedParty.TotalBudget;
        party.RemainingBudget = updatedParty.RemainingBudget;
        party.Date = updatedParty.Date;
        party.LocationId = updatedParty.LocationId;
        party.FoodMenuId = updatedParty.FoodMenuId;

        _context.SaveChanges();

        return RedirectToAction("PartyDetails", new { id = party.Id });
    }

    // GET: DeleteParty (Confirmare)
    [HttpGet]
    public IActionResult DeleteParty(int id)
    {
        var party = _context.Parties.FirstOrDefault(p => p.Id == id);

        if (party == null)
        {
            return NotFound();
        }

        return View(party);
    }

    // POST: DeleteParty
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePartyConfirmed(int id)
    {
        // Găsește petrecerea
        var party = _context.Parties.FirstOrDefault(p => p.Id == id);
        if (party == null)
        {
            return NotFound();
        }

        // Șterge înregistrările asociate din PartyPartiers
        var partyPartiers = _context.PartyPartiers.Where(pp => pp.PartyId == id).ToList();
        _context.PartyPartiers.RemoveRange(partyPartiers);

        // Șterge petrecerea
        _context.Parties.Remove(party);

        // Salvează modificările
        _context.SaveChanges();

        return RedirectToAction("Dashboard");
    }
    [HttpGet]
    public IActionResult AddMember(int partyId)
    {
        var party = _context.Parties.FirstOrDefault(p => p.Id == partyId);
        if (party == null)
        {
            return NotFound("Petrecerea nu a fost găsită.");
        }

        ViewBag.PartyId = partyId;
        ViewBag.PartyName = party.Name;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddMember(int partyId, string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["ErrorMessage"] = "Adresa de email nu poate fi goală.";
            return RedirectToAction("AddMember", new { partyId });
        }

        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Nu există niciun utilizator cu această adresă de email.";
            return RedirectToAction("AddMember", new { partyId });
        }

        var partier = _context.Partiers.FirstOrDefault(p => p.UserId == user.Id);
        if (partier == null)
        {
            TempData["ErrorMessage"] = "Utilizatorul găsit nu este un partier.";
            return RedirectToAction("AddMember", new { partyId });
        }

        var alreadyMember = _context.PartyPartiers.Any(pp => pp.PartyId == partyId && pp.PartierId == partier.Id);
        if (alreadyMember)
        {
            TempData["ErrorMessage"] = "Partierul este deja membru al acestei petreceri.";
            return RedirectToAction("AddMember", new { partyId });
        }

        var partyPartier = new PartyPartierModel
        {
            PartyId = partyId,
            PartierId = partier.Id
        };

        _context.PartyPartiers.Add(partyPartier);
        _context.SaveChanges();

        UpdatePartyTotalPoints(partyId);

        TempData["SuccessMessage"] = "Partierul a fost adăugat cu succes și totalul punctelor a fost actualizat.";

        return RedirectToAction("PartyDetails", new { id = partyId });
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveMember(int partyId, int partierId)
    {
        var partyPartier = _context.PartyPartiers
            .FirstOrDefault(pp => pp.PartyId == partyId && pp.PartierId == partierId);

        if (partyPartier == null)
        {
            return NotFound();
        }

        _context.PartyPartiers.Remove(partyPartier);
        _context.SaveChanges();

        UpdatePartyTotalPoints(partyId);
        TempData["SuccessMessage"] = "Membrul a fost șters și totalul punctelor a fost actualizat.";


        return RedirectToAction("PartyDetails", new { id = partyId });
    }



    private void UpdatePartyTotalPoints(int partyId)
    {

        var party = _context.Parties
            .Include(p => p.PartyUsers) // relația PartyPartiers
            .ThenInclude(pp => pp.Partier)
            .FirstOrDefault(p => p.Id == partyId);

        if (party == null)
        {
            throw new Exception("Petrecerea nu a fost găsită.");
        }
        int totalPoints = party.PartyUsers.Sum(pu => pu.Partier.Points);
        party.TotalPoints = totalPoints;
        _context.SaveChanges();
    }

    [HttpGet]
    public IActionResult PartyTasks(int id)
    {
        // Preluăm petrecerea din baza de date, incluzând task-urile asociate
        var tasks = _context.Tasks.Where(x => x.PartyId == id).ToList();

        if (tasks == null)
        {
            return NotFound(); // Dacă nu găsim petrecerea, returnăm un 404
        }

        // Trimitem task-urile către view
        return View(tasks);
    }


    // Funcție pentru a marca un task ca finalizat
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CompleteTask(int taskId, int partyId)
    {
        var currentUserId = GetCurrentUserId();
        var currentPartier = _context.Partiers.FirstOrDefault(p => p.UserId == currentUserId);

        if (currentPartier == null)
        {
            TempData["ErrorMessage"] = "Nu ai permisiunea necesară.";
            return RedirectToAction("PartyDetails", new { id = partyId });
        }

        var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId && t.PartierId == currentPartier.Id);

        if (task == null || task.IsCompleted)
        {
            TempData["ErrorMessage"] = "Task-ul nu există sau este deja completat.";
            return RedirectToAction("PartyDetails", new { id = partyId });
        }

        // Marchează task-ul ca completat și actualizează punctele
        task.IsCompleted = true;
        currentPartier.Points += task.Points;

        _context.SaveChanges();

        TempData["SuccessMessage"] = "Task-ul a fost marcat ca finalizat!";
        return RedirectToAction("PartyDetails", new { id = partyId });
    }

    // Funcție pentru a lua un task neasignat
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AssignTask(int taskId, int partyId)
    {
        var currentUserId = GetCurrentUserId();
        var currentPartier = _context.Partiers.FirstOrDefault(p => p.UserId == currentUserId);

        if (currentPartier == null)
        {
            TempData["ErrorMessage"] = "Nu ești asociat cu această petrecere!";
            return RedirectToAction("PartyDetails", new { id = partyId });
        }

        var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId && t.PartierId == null);

        if (task == null)
        {
            TempData["ErrorMessage"] = "Task-ul este deja asignat.";
            return RedirectToAction("PartyDetails", new { id = partyId });
        }

        // Asignează task-ul utilizatorului logat
        task.PartierId = currentPartier.Id;
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Ai preluat task-ul!";
        return RedirectToAction("PartyDetails", new { id = partyId });
    }

    [HttpGet]
    public IActionResult AddTask(int partyId)
    {
        // Transmite partyId în view pentru a ști la ce petrecere se adaugă taskul
        ViewBag.PartyId = partyId;

        return View();
    }

    [HttpPost]
    public IActionResult AddTask(int partyId, TaskModel task)
    {
        if (ModelState.IsValid)
        {
            // Asociază taskul cu partyId
            task.PartyId = partyId;
            //task.Party = partyId;
            // Adaugă taskul în baza de date
            _context.Tasks.Add(task);
            _context.SaveChanges();

            // Redirecționează către lista de taskuri a petrecerii
            return RedirectToAction("PartyTasks", new { id = partyId });
        }
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError(error.ErrorMessage);
            }
        }


        // Dacă formularul nu e valid, retrimite view-ul
        ViewBag.PartyId = partyId;
        return RedirectToAction("PartyTasks", new { id = partyId });

    }


}
