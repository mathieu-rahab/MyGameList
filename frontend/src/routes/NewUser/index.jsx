import { Link } from "react-router";

export default function NewUser(){
    return (
        <div>
            <h1>Créer un compte</h1>
            <div>
                <strong>
                    Pseudo
                </strong>
                <input type="text"/>

                <strong>
                    E-mail
                </strong>
                <input type="text"/>

                <strong>
                    Mot de Passe
                </strong>
                <input type="text"/>

                <strong>
                    Confirme le Mot de Passe
                </strong>
                <input type="text"/>

                <button>Valider</button>
            </div>

            <div>
                <i>Déjà un compte?</i> <Link to="/Login">Connecte toi</Link>

            </div>
            
        </div>
    )
}