using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auctria_Application.Controllers.v1;

// Authorize with Admin and SuperAdmin
[Authorize(Roles = "SuperAdmin, Admin")]
[Route("api/Admin/[controller]")]
public class BaseAdminController : BaseApiController
{
    
}