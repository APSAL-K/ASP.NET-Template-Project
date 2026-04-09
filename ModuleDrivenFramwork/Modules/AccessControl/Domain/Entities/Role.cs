using System;
using System.Collections.Generic;

namespace ModuleDrivenFramwork.Modules.AccessControl.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
