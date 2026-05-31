import { Link } from "react-router";
import {useNavigate, useLocation, useSearchParams} from 'react-router-dom';
import { useState } from "react";
import "./index.css"
import "../../auth.css"
import { useTranslation } from "react-i18next";
import { getServerErrorMessage, getHttpErrorMessage } from '../../api/errorHandler.js';
import {useAuth} from "../../utils/useAuth.jsx";
import { useUserService } from "../../api/userService.js";


export default function Login(){
    const {t, i18n} = useTranslation();
    const { user: _, loading: __ } = useAuth();
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const userService = useUserService();
    const [inputs, setInputs] = useState({
        email: useLocation().state?.email || '', 
        password: ""
    });
    const [errors, setErrors] = useState({});
    const [loading, setLoading] = useState(false);


    const handleChange = (e) => {
        const { name, value } = e.target;
        const newInputs = { ...inputs, [name]: value };
        setInputs(newInputs);
    };


    function handleSubmit(e) {
        e.preventDefault();
        // Marquer tout comme touché pour afficher toutes les erreurs
        setLoading(true);

        userService.login(inputs.email, inputs.password)
            .then(() => {
                const redirectUrl = searchParams.get('redirect');
                // Après connexion réussie
                if (redirectUrl) {
                    navigate(decodeURIComponent(redirectUrl));
                } else {
                    navigate('/');
                }
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
                               value={inputs.email} onChange={handleChange}  required />
                    </div>
                    <div className="input-wrap">
                        <i className="ti ti-lock" aria-hidden="true"></i>
                        <input type="password" name="password" placeholder={t('Login.Password')}
                               value={inputs.password} onChange={handleChange}  required />
                    </div>
                    {errors.server && (
                            <div className="error-container">
                                <i className="ti ti-alert-circle" aria-hidden="true"></i>
                                {errors.server}
                            </div>
                        )}
                    <button type="submit" className="auth-submit" disabled={loading}>{t('Login.Submit')}</button>
                </form>



                <div className="auth-alternative">
                    {t('Login.NoAccountYetQuestion')} <Link to="/NewUser">{t('Login.CreateAccount')}</Link>
                </div>

            </div>
        </div>
    );
    
}