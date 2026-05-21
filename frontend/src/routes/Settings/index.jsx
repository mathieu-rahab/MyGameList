import { useState } from "react";
import "./index.css"
import "../../auth.css"
import { useTranslation } from "react-i18next";
import { useUserService } from "../../api/userService";
import { useAuth } from "../../utils/useAuth";


export default function Settings() {

    const userService = useUserService();
    const { user, loading: authLoading } = useAuth();

    const PSEUDO_REGEX = /^[a-zA-Z0-9_\-.]+$/;
    const EMAIL_REGEX  = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    const {t} = useTranslation();
    const [errors, setErrors] = useState({});

    /*
    /// USERNAME SECTION
    */
    
    const [newPseudo, setNewPseudo] = useState("");

    const validatePseudo = (values) => {
        const errs = {};
        if (values.length < 3) errs.pseudo = t('CreateAccount.Validation.PseudoTooShort');
        else if (values.length > 20) errs.pseudo = t('CreateAccount.Validation.PseudoTooLong');
        else if(!PSEUDO_REGEX.test(values))
            errs.pseudo = t('CreateAccount.Validation.PseudoInvalidCharacter');

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
                setErrors('User ID not found');
                setLoading(false);
                return;
            }
            await userService.changePseudo(userId, newPseudo);

            setNewPseudo("");

        } catch (err) {
            console.error(err);
        }
    }

    /*
    /// EMAIL SECTION
    */

    const [newEmail, setNewEmail] = useState("");

    const validateEmail = (values) => {
        const errs = {};
        if(!EMAIL_REGEX.test(values))
            errs.email = t('CreateAccount.Validation.EmailInvalid');

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
                setLoading(false);
                return;
            }

            await userService.changeEmail(newEmail);

            setNewEmail("");
        
        } catch (err) {
            console.error(err);
        }
    }

    /*
    /// PASSWORD SECTION
    */
    const [oldPassword, setOldPassword] = useState("");

    const [newPassword, setNewPassword] = useState("");

    const validatePassword = (values) => {
        const errs = {};
        if (values.length <= 6) errs.password = t('CreateAccount.Validation.PasswordInvalid');
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
                setLoading(false);
                return;
            }

            await userService.changePassword(oldPassword, newPassword);

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
                <span>Bonjour </span>
            </div>

            <div>
                <span>GESTION PSEUDO</span>

                <input type = "text" placeholder='New username' value={newPseudo} onChange={(e) => setNewPseudo(e.target.value)}/>
                <input
                    type="button"
                    value= "Valider"
                    onClick={() => changePseudo()}
                />
            </div>

            <div>
                <span>GESTION EMAIL</span>

                <input type = "text" placeholder='New email' value={newEmail} onChange={(e) => setNewEmail(e.target.value)}/>
                <input
                    type="button"
                    value= "Valider"
                    onClick={() => changeEmail()}
                />
            </div>

            <div>
                <span>GESTION MOT DE PASSE</span>

                <input type = "password" placeholder='Old password' value={oldPassword} onChange={(e) => setOldPassword(e.target.value)}/>

                <input type = "password" placeholder='New password' value={newPassword} onChange={(e) => setNewPassword(e.target.value)}/>
                <input
                    type="button"
                    value= "Valider"
                    onClick={() => changePassword()}
                />
            </div>

        </div>
    );
}