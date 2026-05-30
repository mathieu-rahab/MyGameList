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
    const STEAMID_REGEX = /^\d{17}$/;

    const {t, i18n} = useTranslation();
    const [errors, setErrors] = useState({});

    useEffect(() => {
        if (user) {
            setNewPseudo(user.pseudo || "");
            setNewEmail(user.email || "");
            setNewSteamId(user.steamId || "");
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

        await userService.changePseudo(userId, newPseudo);

        setErrors({});

        try {
            await refreshUser(userId);

            alert(t('Settings.Validation.PopUpOk'));

        } catch (error) {
            alert(t('Settings.Validation.PopUpFailed'));
        }

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

            setErrors({});
            
            try {
                await refreshUser(userId);

                alert(t('Settings.Validation.PopUpOk'));

            } catch (error) {
                alert(t('Settings.Validation.PopUpFailed'));
            }
        
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

            setErrors({});

            try {
                await refreshUser(userId);

                alert(t('Settings.Validation.PopUpOk'));

            } catch (error) {
                alert(t('Settings.Validation.PopUpFailed'));
            }

            setNewPassword("")
            setOldPassword("")
            
        } catch (err) {
            console.error(err);
        }
    }

    /*
    /// STEAMID SECTION
    /// 76561198000000000
    */

    const [newSteamId, setNewSteamId] = useState("");

    const validateSteamId = (values) => {
        const errs = {};
        if(!STEAMID_REGEX.test(values))
            errs.steamId = t('Settings.Validation.SteamIdInvalid');
        else if (values === user?.steamId) {
            errs.steamId = t("Settings.Validation.SteamIdNotChanged");
        }

        return errs;
    };

    async function changeSteamId() {
        const errors = validateSteamId(newSteamId);

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

            await userService.changeSteamId(userId, newSteamId);

            setErrors({});
            
            try {
                await refreshUser(userId);

                alert(t('Settings.Validation.PopUpOk'));

            } catch (error) {
                alert(t('Settings.Validation.PopUpFailed'));
            }
        
        } catch (err) {
            console.log(err);

            // erreur backend connue
            if (err.error) {
                setErrors(prev => ({ ...prev, steamId: getServerErrorMessage(err.error, t, i18n, 'Settings') }));
                
                return;
            }

            // erreur HTTP/réseau
            setErrors(prev => ({...prev, steamId: getHttpErrorMessage(err.status, t)}));
        }
    }

    /*
    /// RETURN
    */

    return (
        
        <div className="set-page">

            <div className="changing_pseudo">
                <span>{t('Settings.Validation.setPseudo')}</span>
                    <input type = "text" placeholder={t('Settings.Validation.newPseudo')} value={newPseudo} onChange={(e) => setNewPseudo(e.target.value)}/>
                    
                    {errors.pseudo && (<label htmlFor="pseudo" className="error">{errors.pseudo}</label>)}

                    <input
                        type="button"
                        value= "Valider"
                        onClick={() => changePseudo()}
                    />
            </div>

            <div className="changing_email">
                <span>{t('Settings.Validation.setEmail')}</span>

                <input type = "text" placeholder={t('Settings.Validation.newEmail')} value={newEmail} onChange={(e) => setNewEmail(e.target.value)}/>
                
                {errors.email && (<label htmlFor="email" className="error">{errors.email}</label>)}
                
                <input
                    type="button"
                    value= {t('Settings.Validation.button')}
                    onClick={() => changeEmail()}
                />
            </div>

            <div className="changing_password">
                <span>{t('Settings.Validation.setPassword')}</span>

                <input type = "password" placeholder={t('Settings.Validation.oldPassword')} value={oldPassword} onChange={(e) => setOldPassword(e.target.value)}/>

                <input type = "password" placeholder={t('Settings.Validation.newPassword')} value={newPassword} onChange={(e) => setNewPassword(e.target.value)}/>
                
                {errors.password && (<label htmlFor="password" className="error">{errors.password}</label>)}
                
                <input
                    type="button"
                    value= {t('Settings.Validation.button')}
                    onClick={() => changePassword()}
                />
            </div>

            <div className="changing_steamid">
                <span>{t('Settings.Validation.setSteamId')}</span>

                <span id="steam_account">{t('Settings.Steam.Texte1')} <a href="https://store.steampowered.com/account/" target="_blank">{t('Settings.Steam.Texte2')}</a>.</span>

                <input type = "text" placeholder={t('Settings.Validation.newSteamId')} value={newSteamId} onChange={(e) => setNewSteamId(e.target.value)}/>
                
                {errors.steamId && (<label htmlFor="steamId" className="error">{errors.steamId}</label>)}
                
                <input
                    type="button"
                    value= {t('Settings.Validation.button')}
                    onClick={() => changeSteamId()}
                />
            </div>

        </div>
    );
}