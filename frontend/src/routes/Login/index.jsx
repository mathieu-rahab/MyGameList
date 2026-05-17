import { Link } from "react-router";
import { useContext, useState } from "react";
import "./index.css"
import "../../auth.css"
import { useTranslation } from "react-i18next";

export default function Login(){
    const {t} = useTranslation();

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

                <h1 className="auth-title">{t('Login.Title')}</h1>

                <form className="auth-form" onSubmit={handleSubmit}>
                    <div className="input-wrap">
                        <i className="ti ti-mail" aria-hidden="true"></i>
                        <input type="text" name="email" placeholder="Email"
                               value={inputs.email} onChange={handleChange} />
                    </div>
                    <div className="input-wrap">
                        <i className="ti ti-lock" aria-hidden="true"></i>
                        <input type="password" name="password" placeholder={t('Login.Password')}
                               value={inputs.password} onChange={handleChange} />
                    </div>
                    <button type="submit" className="auth-submit">{t('Login.Submit')}</button>
                </form>

                <div className="auth-divider" style={{display: "none"}}>
                    <div className="auth-divider-line"></div>
                    <span className="auth-divider-text">{t('Login.Alternative')}</span> /*ou continuer avec*/
                    <div className="auth-divider-line"></div>
                </div>

                <button className="btn-steam" style={{display: "none"}}>
                    <i className="ti ti-brand-steam" aria-hidden="true"></i>
                    Steam
                </button>

                <div className="auth-alternative">
                    {t('Login.NoAccountYetQuestion')} <Link to="/NewUser">{t('Login.CreateAccount')}</Link>
                </div>

            </div>
        </div>
    );
    
}