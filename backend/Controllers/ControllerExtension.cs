using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using prid_2021_a06.Models;
using System.ComponentModel.DataAnnotations;
using PRID_Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Security.Claims;
using prid_2021_a06.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using prid_2021_a06.Controllers;
namespace prid_2021_a06.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public static class ControllerExtension {
        public static User GetCurrentUser(this ControllerBase ctl, PridContext ctx) {
            var name = ctl.User.Identity.Name;
            var user = ctx.Users.SingleOrDefault(u => u.Pseudo == name);
            return user;
        }
    }
}