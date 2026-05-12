import { useEffect, useState } from "react";
import { Link, Outlet } from "react-router";
import "./index.css";
import { useTranslation } from "react-i18next";

export default function Layout() {
    //traduction
    const {t} = useTranslation();
    //

    const [theme, setTheme] = useState(() => {
        try { return localStorage.getItem('nx-theme') || 'dark'; }
        catch { return 'dark'; }
    });

    // Synchronise la classe sur <html> à chaque changement de thème
    useEffect(() => {
        const root = document.documentElement;
        root.classList.remove('dark', 'light');
        root.classList.add(theme);
        try { localStorage.setItem('nx-theme', theme); } catch {}
    }, [theme]);

    const [menuOpen, setMenuOpen] = useState(false);

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
                    <Link to="/dashboard"><button>{t('Layout.Dashboard')}</button></Link>
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
                    <div className="notif">⊹</div>
                    <Link to="/login">
                        <button className="btn-login">{t('Layout.Login')}</button>
                    </Link>
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
                    <div className="fcopy">MyGameList ® 2026</div>
                </div>
            </footer>
        </div>
    );
}