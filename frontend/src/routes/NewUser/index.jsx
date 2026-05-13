import { Link } from "react-router";
import { useContext, useState } from "react";
import "./index.css"
import "../../auth.css"
import { useTranslation } from "react-i18next";


export default function NewUser(){
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

                <h1 className="auth-title">{t('CreateAccount.Title')}</h1>

                <form className="auth-form" onSubmit={handleSubmit}>
                    <div className="input-wrap">
                        <i className="ti ti-user" aria-hidden="true"></i>
                        <input type="text" name="pseudo" placeholder="Pseudo"
                               value={inputs.pseudo} onChange={handleChange} />
                    </div>
                    <div className="input-wrap">
                        <i className="ti ti-mail" aria-hidden="true"></i>
                        <input type="text" name="email" placeholder="Email"
                               value={inputs.email} onChange={handleChange} />
                    </div>
                    <div className="input-wrap">
                        <i className="ti ti-lock" aria-hidden="true"></i>
                        <input type="password" name="password" placeholder={t('CreateAccount.Password')}
                               value={inputs.password} onChange={handleChange} />
                    </div>
                    <div className="input-wrap">
                        <i className="ti ti-lock-check" aria-hidden="true"></i>
                        <input type="password" name="passwordConfirm" placeholder={t('CreateAccount.ConfirmPassword')}
                               value={inputs.passwordConfirm} onChange={handleChange} />
                    </div>
                    <button type="submit" className="auth-submit">{t('CreateAccount.Submit')}</button>
                </form>

                <div className="auth-alternative">
                    {t('CreateAccount.ExistingAccountQuestion')}<Link to="/Login">{t('CreateAccount.ToLogin')}</Link>
                </div>

            </div>
        </div>
    );


}







