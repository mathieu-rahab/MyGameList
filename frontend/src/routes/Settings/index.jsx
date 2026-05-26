import { useState, useEffect } from "react";
import "./index.css"
import "../../auth.css"
import { useTranslation } from "react-i18next";
import { useUserService } from "../../api/userService";
import { useAuth } from "../../utils/useAuth";
import { getServerErrorMessage, getHttpErrorMessage } from '../../api/errorHandler.js';


export default function Settings() {

    const userService = useUserService();
    const { user, loadingloading: authLoading, refreshUser } = useAuth();

    const PSEUDO_REGEX = /^[a-zA-Z0-9_\-.]+$/;
    const EMAIL_REGEX  = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    const {t, i18n} = useTranslation();
    const [errors, setErrors] = useState({});

    useEffect(() => {
        if (user) {
            setNewPseudo(user.pseudo || "");
            setNewEmail(user.email || "");
        }
    }, [user]);

    /*
    /// USERNAME SECTION
    */
    
    const [newPseudo, setNewPseudo] = useState("");

    const validatePseudo = (values) => {
    const errs = {};

    if (values.length < 3) {
        errs.pseudo = t('Settings.Validation.PseudoTooShort');
    }
    else if (values.length > 20) {
        errs.pseudo = t('Settings.Validation.PseudoTooLong');
    }
    else if (!PSEUDO_REGEX.test(values)) {
        errs.pseudo = t('Settings.Validation.PseudoInvalidCharacter');
    }
    else if (values === user?.pseudo) {
        errs.pseudo = t("Settings.Validation.PseudoNotChanged");
    }

    return errs;
};

    async function changePseudo() {

    const errors = validatePseudo(newPseudo);

    if (Object.keys(errors).length > 0) {
        setErrors(errors);
        return;
    }

    try {

        const userId = user?.userId;

        if (!userId) {
            setErrors({ pseudo: 'User ID not found' });
            return;
        }

        setErrors({});

        await userService.changePseudo(userId, newPseudo);

        await refreshUser(userId);

    } catch (err) {

        console.log(err);

        // erreur backend connue
        if (err.error) {
            setErrors(prev => ({ ...prev, pseudo: getServerErrorMessage(err.error, t, i18n, 'CreateAccount') }));
            return;
        }

        // erreur HTTP/réseau
        setErrors(prev => ({...prev, pseudo: getHttpErrorMessage(err.status, t)}));
    }
}

    /*
    /// EMAIL SECTION
    */

    const [newEmail, setNewEmail] = useState("");

    const validateEmail = (values) => {
        const errs = {};
        if(!EMAIL_REGEX.test(values))
            errs.email = t('Settings.Validation.EmailInvalid');
        else if (values === user?.email) {
            errs.email = t("Settings.Validation.EmailNotChanged");
}

        return errs;
    };

    async function changeEmail() {
        const errors = validateEmail(newEmail);

        if (Object.keys(errors).length > 0) {
            setErrors(errors);
            return;
        }

        try {
            const userId = user?.userId;
            if (!userId) {
                setErrors('User ID not found');
                return;
            }

            await userService.changeEmail(userId, newEmail);
            
            await refreshUser(userId);
        
        } catch (err) {
            console.log(err);

            // erreur backend connue
            if (err.error) {
                setErrors(prev => ({ ...prev, email: getServerErrorMessage(err.error, t, i18n, 'CreateAccount') }));
                return;
            }

            // erreur HTTP/réseau
            setErrors(prev => ({...prev, email: getHttpErrorMessage(err.status, t)}));
        }
    }

    /*
    /// PASSWORD SECTION
    */
    const [oldPassword, setOldPassword] = useState("");

    const [newPassword, setNewPassword] = useState("");

    const validatePassword = (values) => {
        const errs = {};
        if (values.length <= 6) errs.password = t('Settings.Validation.PasswordInvalid');
        /*
        if (values !== values.passwordConfirm)
            errs.passwordConfirm = t('CreateAccount.Validation.PasswordConfirmDifferent');
        */
        return errs;
    };

    async function changePassword() {

        const errors = validatePassword(newPassword);

        if (Object.keys(errors).length > 0) {
            setErrors(errors);
            return;
        }

        try {
            const userId = user?.userId;
            if (!userId) {
                setErrors('User ID not found');
                return;
            }

            await userService.changePassword(userId, oldPassword, newPassword);

            await refreshUser(userId);

            setNewPassword("")
            setOldPassword("")
            
        } catch (err) {
            console.error(err);
        }
    }

    /*
    /// RETURN
    */

    return (
        
        <div className="set-page">
            <div className="profil">
                <span>Bonjour {user?.pseudo}</span>
            </div>

            <div className="changing_pseudo">
                <span>GESTION PSEUDO</span>
                    <input type = "text" placeholder='New username' value={newPseudo} onChange={(e) => setNewPseudo(e.target.value)}/>
                    
                    {errors.pseudo && (<label htmlFor="pseudo" className="error">{errors.pseudo}</label>)}

                    <input
                        type="button"
                        value= "Valider"
                        onClick={() => changePseudo()}
                    />
            </div>


            <div className="changing_email">
                <span>GESTION EMAIL</span>

                <input type = "text" placeholder='New email' value={newEmail} onChange={(e) => setNewEmail(e.target.value)}/>
                
                {errors.email && (<label htmlFor="email" className="error">{errors.email}</label>)}
                
                <input
                    type="button"
                    value= "Valider"
                    onClick={() => changeEmail()}
                />
            </div>

            <div className="changing_password">
                <span>GESTION MOT DE PASSE</span>

                <input type = "password" placeholder='Old password' value={oldPassword} onChange={(e) => setOldPassword(e.target.value)}/>

                <input type = "password" placeholder='New password' value={newPassword} onChange={(e) => setNewPassword(e.target.value)}/>
                
                {errors.password && (<label htmlFor="password" className="error">{errors.password}</label>)}
                
                <input
                    type="button"
                    value= "Valider"
                    onClick={() => changePassword()}
                />
            </div>

        </div>
    );
}