import { 
  Users, 
  ShieldCheck, 
  Key, 
  LogOut, 
  LayoutDashboard, 
  ShoppingBag, 
  Package, 
  Settings,
  UserCircle 
} from 'lucide-react'

interface SidebarProps {
  currentSection: string;
  onSectionChange: (section: string) => void;
  userEmail: string;
  roles: string[];
  permissions: string[];
  onSignOut: () => void;
}

export function Sidebar({ currentSection, onSectionChange, userEmail, roles, permissions, onSignOut }: SidebarProps) {
  const isAdmin = roles.includes('Admin') || roles.includes('SuperAdmin');

  const sections = [
    { id: 'overview', name: 'Overview', icon: LayoutDashboard, category: 'Main' },
    { id: 'users', name: 'User Management', icon: Users, category: 'Administration', permission: 'auth.users.manage' },
    { id: 'roles', name: 'Role Definitions', icon: ShieldCheck, category: 'Administration', permission: 'auth.users.manage' },
    { id: 'permissions', name: 'Access Control', icon: Key, category: 'Administration', permission: 'auth.users.manage' },
    { id: 'settings', name: 'System Settings', icon: Settings, category: 'Administration', permission: 'auth.users.manage' },
    { id: 'products', name: 'Product Catalog', icon: Package, category: 'E-commerce', disabled: true },
    { id: 'orders', name: 'Order Processing', icon: ShoppingBag, category: 'E-commerce', disabled: true },
  ];

  // Filter sections based on Admin status or specific permissions
  const filteredSections = sections.filter(section => {
    // Overview is always visible
    if (section.id === 'overview') return true;
    
    // Admins see everything
    if (isAdmin) return true;

    // Others check specific permission
    if (section.permission) {
      return permissions.includes(section.permission);
    }

    return true;
  });

  const categories = Array.from(new Set(filteredSections.map(s => s.category)));

  return (
    <aside className="sidebar">
      <div className="sidebar-header">
        <div className="sidebar-brand-wrapper">
          <span className="sidebar-brand"> Console App</span>
        </div>
      </div>

      <nav className="sidebar-nav">
        {categories.map(category => (
          <div key={category} className="nav-section">
            <span className="nav-section-label">{category}</span>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
              {filteredSections
                .filter(s => s.category === category)
                .map(section => (
                  <button
                    key={section.id}
                    className={`nav-link ${currentSection === section.id ? 'active' : ''}`}
                    onClick={() => !section.disabled && onSectionChange(section.id)}
                    disabled={section.disabled}
                    style={{ 
                      width: '100%', 
                      border: 'none', 
                      background: 'none', 
                      cursor: section.disabled ? 'not-allowed' : 'pointer', 
                      opacity: section.disabled ? 0.3 : 1 
                    }}
                  >
                    <section.icon size={20} strokeWidth={2.5} />
                    <span style={{ paddingTop: '1px' }}>{section.name}</span>
                  </button>
                ))}
            </div>
          </div>
        ))}
      </nav>

      <div className="sidebar-footer">
        <div className="user-profile-card">
          <div className="user-avatar" style={{ minWidth: '40px', height: '40px', background: 'var(--accent-soft)', borderRadius: '12px', display: 'flex', alignItems: 'center', justifyContent: 'center', color: 'var(--accent-strong)' }}>
            <UserCircle size={26} strokeWidth={2.5} />
          </div>
          <div className="user-info" style={{ overflow: 'hidden', flex: 1 }}>
            <span className="user-name" style={{ fontWeight: 800, fontSize: '0.9rem', color: 'var(--text)', display: 'block', marginBottom: '2px' }}>
              {userEmail.split('@')[0]}
            </span>
            <span className="user-email" style={{ fontSize: '0.725rem', color: 'var(--muted)', display: 'block', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
              {userEmail}
            </span>
          </div>
          <button 
            onClick={onSignOut}
            className="btn-ghost"
            style={{ padding: '6px', borderRadius: '8px', color: 'var(--danger)' }}
            title="Sign Out"
          >
            <LogOut size={18} />
          </button>
        </div>
      </div>
    </aside>
  )
}
