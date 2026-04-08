import { useContext } from "react";
import { Link, Outlet } from "react-router";

export default function Layout(){
    return (
        <>
            <ul>
                <li><Link to="/">Acceuil</Link></li>
                <li><Link to="/login">Se connecter</Link></li>
                <li><Link to="/NewUser">Créer un compte</Link></li>
            </ul>
            <Outlet/>
            <footer>
                <span>MyGameList ® 2026</span>
            </footer>

        </>
    )
}