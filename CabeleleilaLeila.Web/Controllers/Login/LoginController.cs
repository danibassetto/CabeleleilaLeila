﻿using CabeleleilaLeila.Arguments;
using CabeleleilaLeila.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CabeleleilaLeila.Controllers;

public class LoginController(IUserServiceClient userServiceClient, Web.Helpers.ISession session) : Controller
{
    private readonly IUserServiceClient _userServiceClient = userServiceClient;
    private readonly Web.Helpers.ISession _session = session;

    public IActionResult Index()
    {
        if (_session.GetUserSession() != null)
            return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(InputLoginUser input)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var response = await _userServiceClient.Login(input);

                if (response.Success)
                {
                    var user = response.Data;
                    _session.CreateUserSession(JsonConvert.SerializeObject(user));
                    return RedirectToAction("Index", "Home");
                }

                TempData["ErrorMessage"] = response.ErrorMessage!;
                return RedirectToAction("Index");
            }

            return View("Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Erro: {ex.Message}";
            return RedirectToAction("Index");
        }
    }

    public IActionResult Exit()
    {
        _session.RemoveUserSession();
        return RedirectToAction("Index", "Login");
    }

    public IActionResult SendLinkToRedefinePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SendLinkToRedefinePassword(InputSendLinkToRedefinePasswordUser input)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var response =  await _userServiceClient.SendLinkToRedefinePassword(input);

                if (response.Success)                
                    return RedirectToAction("Index", "Login");                

                TempData["ErrorMessage"] = response.ErrorMessage!;
            }

            return View("Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Erro: {ex.Message}";
            return RedirectToAction("Index");
        }
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(InputCreateUser input)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var response = await _userServiceClient.Create(input);

                if (response.Success)
                    return RedirectToAction("Index");

                TempData["ErrorMessage"] = response.ErrorMessage!;
            }

            return View(input);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Erro: {ex.Message}";
            return RedirectToAction("Index");
        }
    }
}