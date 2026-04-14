import { useContext, useState } from "react";
import { Link, Outlet } from "react-router";
import "./index.css"

export default function Layout(){
    
    return (
        <div id="page">
            <header id="menu">
                <Link to="/"><button id="acceuil">Accueil</button></Link>
                <div>
                    <Link to="/login"><button>Se connecter</button></Link>
                    <Link to="/NewUser"><button>Créer un compte</button></Link>
                </div>
                
            </header>
            <div id="Outlet">
                <Outlet/>
            </div>
            <footer>
                <span>MyGameList ® 2026</span>
            </footer>

        </div>
    )
}