import { useEffect, useState } from "react";
import {Link, Outlet} from "react-router";
import "./index.css";
import { useTranslation } from "react-i18next";
import { useAuth } from '../../utils/useAuth'
import { useNavigate } from "react-router-dom";



export default function Layout() {
    const { isAuthenticated, logout } = useAuth();
    const { t, i18n } = useTranslation();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/');
    };

    const LoginButton = () => {
      if (isAuthenticated) {
        return (
            <Link to="/">
                <button onClick={handleLogout} className="btn-login">{t('Layout.Logout')}</button>
            </Link>
        )
      }else{
        return (
            <Link to="/login">
                <button className="btn-login">{t('Layout.Login')}</button>
            </Link>
        )
      }
    }

    const AccountSettingsButton = () => {
      if (isAuthenticated) {
        return(
            <Link to={'/Settings'} className="button-no-style">
                <div className="av">
                        <i className="ti ti-user" aria-hidden="true"></i>
                    </div>
                    <div
                        className="hamburger"
                        aria-label="Menu"
                        onClick={() => setMenuOpen(o => !o)}
                    >
                    <span></span><span></span><span></span>
                </div>
            </Link>
        )
        
      }
        
      
    } 

    const DashboardButton = () => {
      if (isAuthenticated) {
        return (
            <Link to="/dashboard"><button>{t('Layout.Dashboard')}</button></Link>
        )
      }
    }
    

    const [theme, setTheme] = useState(() => {
        try { return localStorage.getItem('nx-theme') || 'dark'; }
        catch { return 'dark'; }
    });

    // Synchronise la classe sur <html> à chaque changement de thème
    useEffect(() => {
        const root = document.documentElement;
        root.classList.remove('dark', 'light');
        root.classList.add(theme);
        try { localStorage.setItem('nx-theme', theme); } catch { /* */ }
    }, [theme]);

    const [menuOpen, setMenuOpen] = useState(false);

    const changeLanguage = (lng) => {
        i18n.changeLanguage(lng);
    };

    const currentLang = i18n.language || 'fr';

    return (
        <div id="page" className="page-bg">
            <div className="orb orb1"></div>
            <div className="orb orb2"></div>
            <div className="orb orb3"></div>

            <header>
                <div className="logo">
                    <div className="logo-dot"></div>
                    MygameList
                </div>
                <nav>
                    <Link to="/"><button className="active">{t('Layout.Home')}</button></Link>
                    {DashboardButton()}
                </nav>
                <div className="hright">
                    <div className="theme-toggle">
                        <div
                            className={`tog-opt ${theme === 'dark' ? 'on' : ''}`}
                            onClick={() => setTheme('dark')}
                        >
                            {t('Layout.Dark')}
                        </div>
                        <div
                            className={`tog-opt ${theme === 'light' ? 'on' : ''}`}
                            onClick={() => setTheme('light')}
                        >
                            {t('Layout.Light')}
                        </div>
                    </div>
                    <div className="notif" style={{display : 'none'}}>⊹</div>
                    {LoginButton()}
                    {AccountSettingsButton()}
                    
                </div>
            </header>

            {/* Menu mobile drawer */}
            <nav className={`nav-drawer ${menuOpen ? 'open' : ''}`}>
                <Link to="/" onClick={() => setMenuOpen(false)}>{t('Layout.Home')}</Link>
                <Link to="/dashboard" onClick={() => setMenuOpen(false)}>{t('Layout.Dashboard')}</Link>
                <div className="theme-toggle-mobile">
                    <div
                        className={`tog-opt ${theme === 'dark' ? 'on' : ''}`}
                        onClick={() => setTheme('dark')}
                    >
                        {t('Layout.Dark')}
                    </div>
                    <div
                        className={`tog-opt ${theme === 'light' ? 'on' : ''}`}
                        onClick={() => setTheme('light')}
                    >
                        {t('Layout.Light')}
                    </div>
                </div>
            </nav>

            <div id="Outlet">
                <Outlet />
            </div>

            <footer>
                <div className="foot-in">
                    <div className="flogo">NEXPLAY</div>
                    
                    {/* Sélecteur de langue */}
                    <div className="theme-toggle">
                        <div
                            className={`tog-opt ${currentLang.startsWith('fr') ? 'on' : ''}`}
                            onClick={() => changeLanguage('fr')}
                        >
                            FR
                        </div>
                        <div
                            className={`tog-opt ${currentLang.startsWith('en') ? 'on' : ''}`}
                            onClick={() => changeLanguage('en')}
                        >
                            EN
                        </div>
                    </div>

                    <div className="fcopy">MyGameList ® 2026</div>
                </div>
            </footer>
        </div>
    );
}