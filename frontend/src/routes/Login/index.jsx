import { Link } from "react-router";
import { useContext, useState } from "react";
import "./index.css"
import "../../auth.css"

export default function Login(){
    const [inputs, setInputs] = useState({});

    const handleChange = (e) => {  /* TODO: surveiller les inputs*/ 
        const name = e.target.name;
        const value = e.target.value;
        setInputs(values => ({...values, [name]: value}))
    }

    function handleSubmit(e) { /* TODO: gèrer le formulaire*/ 
    e.preventDefault();
    alert("submited");
    }

    return (
        <div className="auth-page">
            <div className="auth-card">

                <h1 className="auth-title">Se <span>connecter</span></h1>

                <form className="auth-form" onSubmit={handleSubmit}>
                    <div className="input-wrap">
                        <i className="ti ti-mail" aria-hidden="true"></i>
                        <input type="text" name="email" placeholder="Email"
                               value={inputs.email} onChange={handleChange} />
                    </div>
                    <div className="input-wrap">
                        <i className="ti ti-lock" aria-hidden="true"></i>
                        <input type="password" name="password" placeholder="Mot de passe"
                               value={inputs.password} onChange={handleChange} />
                    </div>
                    <button type="submit" className="auth-submit">Connexion</button>
                </form>

                <div className="auth-divider" style={{display: "none"}}>
                    <div className="auth-divider-line"></div>
                    <span className="auth-divider-text">ou continuer avec</span>
                    <div className="auth-divider-line"></div>
                </div>

                <button className="btn-steam" style={{display: "none"}}>
                    <i className="ti ti-brand-steam" aria-hidden="true"></i>
                    Steam
                </button>

                <div className="auth-alternative">
                    Pas encore de compte ? <Link to="/NewUser">Créer un compte</Link>
                </div>

            </div>
        </div>
    );
    
}