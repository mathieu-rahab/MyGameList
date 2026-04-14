import { Link } from "react-router";
import { useContext, useState } from "react";
import "./index.css"

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
        <div id="page">
            <h1 id="title">Se Connecter</h1>
            <form id="login" onSubmit={handleSubmit} >
                <input 
                    type="text" 
                    name="pseudo"
                    placeholder="Pseudo"
                    value={inputs.pseudo} 
                    onChange={handleChange}
                />
                
                <input 
                    type="password" 
                    name="password"
                    placeholder="Mot de passe"
                    value={inputs.password} 
                    onChange={handleChange}
                />
                <input id="submit" type="submit" value="Envoyer" />
            </form>
            <div id="alternative">
                <i>Pas encore de compte?</i>&nbsp;<Link to="/NewUser">Crée en un</Link>
            </div>

        </div>
    
  )
    
}