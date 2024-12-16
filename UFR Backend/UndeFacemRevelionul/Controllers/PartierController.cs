﻿using Microsoft.AspNetCore.Mvc;
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

        // 2. Căutăm în baza de date `Partier`-ul care are `UserId` egal cu `currentUserId`
        var partier = _context.Partiers.FirstOrDefault(p => p.UserId == currentUserId);

        // Obține toate petrecerile asociate partierului
        var parties = _context.PartyPartiers
            .Where(p => p.PartierId == partier.Id)
            .Select(p => p.Party)
            .ToList();

        var model = new PartierDashboardViewModel
        {
            Parties = parties
        };

        return View(model);
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
        // Obține ID-ul utilizatorului logat
        var currentUserId = GetCurrentUserId();

        // Căutăm petrecerea după ID
        var party = _context.Parties
            .Include(p => p.PartyUsers)  // Include utilizatorii petrecerii
            .ThenInclude(pu => pu.Partier)  // Include informațiile despre partieri
            .FirstOrDefault(p => p.Id == id);

        if (party == null)
        {
            return NotFound(); // Dacă petrecerea nu există
        }

        // Aici extragem lista de partieri care participă la această petrecere
        var partyPartiers = _context.PartyPartiers
            .Where(pp => pp.PartyId == id)  // Căutăm partierii care sunt asociați acestei petreceri
            .Include(pp => pp.Partier)  // Include datele partierilor
            .ThenInclude(p => p.User)   // Include datele utilizatorilor din Partier
            .Select(pp => pp.Partier)   // Extrage doar Partier-ul
            .ToList();

        // Creăm lista de nume pentru participanți
        var partierNames = partyPartiers.Select(partier => partier.User.Name).ToList();

        // Adăugăm lista de nume a partierilor în petrecere
        ViewBag.PartierNames = partierNames;

        return View("PartyDetails", party); // Asigură-te că se indică corect numele view-ului

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

        TempData["SuccessMessage"] = "Partierul a fost adăugat cu succes.";
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

        return RedirectToAction("PartyDetails", new { id = partyId });
    }









}
