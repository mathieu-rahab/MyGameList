import { Link } from "react-router";
import { useNavigate } from 'react-router-dom';
import { useState } from "react";

import "./index.css"
import "../../auth.css"
import { useTranslation } from "react-i18next";
import { getServerErrorMessage, getHttpErrorMessage } from '../../api/errorHandler.js';
import { useUserService } from "../../api/userService.js";



export default function NewUser(){
    const {t, i18n} = useTranslation();
    const navigate = useNavigate();
    const userService = useUserService();
    const [inputs, setInputs] = useState({
        pseudo: "",
        email: "",
        password: "",
        passwordConfirm: ""
    });
    const [touched, setTouched] = useState({});
    const [errors, setErrors] = useState({});
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);

    const PSEUDO_REGEX = /^[a-zA-Z0-9_\-.]+$/;
    const EMAIL_REGEX  = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;


    const validate = (values) => {
        const errs = {};

        // Pseudo
        if (values.pseudo.length < 3) errs.pseudo = t('CreateAccount.Validation.PseudoTooShort');
        else if (values.pseudo.length > 20) errs.pseudo = t('CreateAccount.Validation.PseudoTooLong');
        else if(!PSEUDO_REGEX.test(values.pseudo))
            errs.pseudo = t('CreateAccount.Validation.PseudoInvalidCharacter');

        // Email
        if(!EMAIL_REGEX.test(values.email))
            errs.email = t('CreateAccount.Validation.EmailInvalid');

        // Passwords
        if (values.password.length <= 6) errs.password = t('CreateAccount.Validation.PasswordInvalid');
        if (values.password !== values.passwordConfirm)
            errs.passwordConfirm = t('CreateAccount.Validation.PasswordConfirmDifferent');

        return errs;
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        const newInputs = { ...inputs, [name]: value };
        setInputs(newInputs);
        setErrors(validate(newInputs));
    };

    const handleBlur = (e) => {
        setTouched(t => ({ ...t, [e.target.name]: true })); // marque comme touché
    };


    function handleSubmit(e) {
        e.preventDefault();
        // Marquer tout comme touché pour afficher toutes les erreurs
        setTouched({ pseudo: true, email: true, password: true, passwordConfirm: true });
        const errs = validate(inputs);
        setErrors(errs);
        if (Object.keys(errs).length > 0) return; // bloque si erreurs
        setLoading(true);
        userService.createUser(inputs.pseudo, inputs.email, inputs.password)
            .then(() => {
                setSuccess(true);
                setTimeout(() => {
                    navigate('/login', { state: { email: inputs.email } });
                }, 3500);
            })
            .catch((err) => {
                setLoading(false);
                console.log(err);
                // erreur backend connue
                if (err.error) {
                    setErrors(prev => ({ ...prev, server: getServerErrorMessage(err.error, t, i18n, 'CreateAccount') }));
                    return;
                }
                // erreur HTTP/réseau
                setErrors(prev => ({ ...prev, server: getHttpErrorMessage(err.status, t) }));
            });
    }

    return (
        <div className="auth-page">
            {success ? (
                <div className="auth-card auth-success">
                    <div className="success-icon">
                        <i className="ti ti-check"></i>
                    </div>
                    <p className="success-title">{t('CreateAccount.Success.Title')}</p>
                    <p className="success-sub">{t('CreateAccount.Success.Subtitle')}</p>
                </div>
            ) : (
                <div className="auth-card">

                    <h1 className="auth-title">{t('CreateAccount.Title')}</h1>

                    <form className="auth-form" onSubmit={handleSubmit}>
                        <div className="input-wrap">
                            <i className="ti ti-user" aria-hidden="true"></i>
                            <input type="text" name="pseudo" id="pseudo" placeholder={t('CreateAccount.Fields.Pseudo')}
                                   value={inputs.pseudo} onChange={handleChange} onBlur={handleBlur} required/>
                        </div>
                        {touched.pseudo && errors.pseudo && (
                            <label htmlFor="pseudo" className="error">{errors.pseudo}</label>
                        )}

                        <div className="input-wrap">
                            <i className="ti ti-mail" aria-hidden="true"></i>
                            <input type="text" name="email" id="email" placeholder={t('CreateAccount.Fields.Email')}
                                   value={inputs.email} onChange={handleChange} onBlur={handleBlur} required />
                        </div>
                        {touched.email && errors.email && (
                            <label htmlFor="email" className="error">{errors.email}</label>
                        )}

                        <div className="input-wrap">
                            <i className="ti ti-lock" aria-hidden="true"></i>
                            <input type="password" name="password" id="password" placeholder={t('CreateAccount.Fields.Password')}
                                   value={inputs.password} onChange={handleChange} onBlur={handleBlur} required />
                        </div>
                        {touched.password && errors.password && (
                            <label htmlFor="password" className="error">{errors.password}</label>
                        )}

                        <div className="input-wrap">
                            <i className="ti ti-lock-check" aria-hidden="true"></i>
                            <input type="password" name="passwordConfirm" id="passwordConfirm" placeholder={t('CreateAccount.Fields.ConfirmPassword')}
                                   value={inputs.passwordConfirm} onChange={handleChange} onBlur={handleBlur} required />
                        </div>
                        {touched.passwordConfirm && errors.passwordConfirm && (
                            <label htmlFor="passwordConfirm" className="error">{errors.passwordConfirm}</label>
                        )}

                        {errors.server && (
                            <div className="error-container">
                                <i className="ti ti-alert-circle" aria-hidden="true"></i>
                                {errors.server}
                            </div>
                        )}

                        <button type="submit" className="auth-submit" disabled={loading}>{t('CreateAccount.Submit')}</button>
                    </form>

                    <div className="auth-alternative">
                        {t('CreateAccount.ExistingAccountQuestion')} <Link to="/Login">{t('CreateAccount.ToLogin')}</Link>
                    </div>

                </div>
            )}
        </div>
    );


}







