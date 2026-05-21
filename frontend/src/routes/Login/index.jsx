import { Link } from "react-router";
import { useNavigate, useLocation } from 'react-router-dom';
import { useCookies } from 'react-cookie';
import { useContext, useState } from "react";
import "./index.css"
import "../../auth.css"
import { useTranslation } from "react-i18next";
import { getServerErrorMessage, getHttpErrorMessage } from '../../api/errorHandler.js';
import {useAuth} from "../../utils/useAuth.jsx";

export default function Login(){
    const {t, i18n} = useTranslation();
    const { user: _, loading: __ } = useAuth();
    const navigate = useNavigate();
    const [cookies, setCookie] = useCookies(['jwt_token']);
    const [inputs, setInputs] = useState({
        email: useLocation().state?.email || '', 
        password: ""
    });
    const [touched, setTouched] = useState({});
    const [errors, setErrors] = useState({});
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);


    const handleChange = (e) => {
        const { name, value } = e.target;
        const newInputs = { ...inputs, [name]: value };
        setInputs(newInputs);
    };

    const handleBlur = (e) => {
        setTouched(t => ({ ...t, [e.target.name]: true })); // marque comme touché
    };

    function handleSubmit(e) {
        e.preventDefault();
        // Marquer tout comme touché pour afficher toutes les erreurs
        setTouched({ email: true, password: true});
        setLoading(true);
        fetch('/api/Identity/token', {
            method: 'POST',
            body: JSON.stringify({
                userEmail: inputs.email,
                password: inputs.password
            }),
            headers: {
                'Content-type': 'application/json; charset=UTF-8',
            },
        })
            .then(async (response) => {
                if (!response.ok) {
                    let err = null;
                    try {
                        err = await response.json();
                    } catch {}
                    return Promise.reject({
                        status: response.status,
                        error: err?.error
                    });
                }
                setSuccess(true);
                const token = await response.text();
                // Définir le cookie
                setCookie('jwt_token', token, { 
                  path: '/',            // Rend le cookie accessible sur tout le site
                  maxAge: 3600,         // Expire dans 1h (en secondes)
                  secure: true,         // HTTPS uniquement
                  sameSite: 'strict'    // Protection CSRF
                });
                setTimeout(() => {
                    navigate('/');
                }, 1000);

            })
            .catch((err) => {
                setLoading(false);
                console.log(err);
                // erreur backend connue
                if (err.error) {
                    setErrors(prev => ({ ...prev, server: getServerErrorMessage(err.error, t, i18n, 'Login') }));
                    return;
                }
                // erreur HTTP/réseau
                setErrors(prev => ({ ...prev, server: getHttpErrorMessage(err.status, t) }));
            });
    }

    return (
        <div className="auth-page">
            <div className="auth-card">

                <h1 className="auth-title">{t('Login.Title')}</h1>

                <form className="auth-form" onSubmit={handleSubmit}>
                    <div className="input-wrap">
                        <i className="ti ti-mail" aria-hidden="true"></i>
                        <input type="text" name="email" placeholder="Email"
                               value={inputs.email} onChange={handleChange} onBlur={handleBlur} required />
                    </div>
                    <div className="input-wrap">
                        <i className="ti ti-lock" aria-hidden="true"></i>
                        <input type="password" name="password" placeholder={t('Login.Password')}
                               value={inputs.password} onChange={handleChange} onBlur={handleBlur} required />
                    </div>
                    {errors.server && (
                            <div className="error-container">
                                <i className="ti ti-alert-circle" aria-hidden="true"></i>
                                {errors.server}
                            </div>
                        )}
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