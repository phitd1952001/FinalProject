﻿using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Controller]
public abstract class BaseController : ControllerBase
{
    // returns the current authenticated account (null if not logged in)
    public Account Account => (Account)HttpContext.Items["Account"];
}