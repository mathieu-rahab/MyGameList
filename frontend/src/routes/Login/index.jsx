import { Link } from "react-router";

export default function Login(){
    return (
        <div>
            <h1>Se Connecter</h1>
            <div>
                <strong>
                    Nom d'Utilisateur
                </strong>
                <input type="text"/>

                <strong>
                    Mot de Passe
                </strong>
                <input type="text"/>

                <button>Valider</button>
            </div>

            <div>
                <i>Pas encore de compte?</i> <Link to="/newUser">Crée en un maintenant</Link>

            </div>
            
        </div>
    )
}