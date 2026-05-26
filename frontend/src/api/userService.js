import { useApiCall } from './apiClient';
import {useCookies} from "react-cookie";
import {storeToken} from "../utils/tokenUtils.js";

export const useUserService = () => {
    const apiCall = useApiCall();
    const [, setCookie] = useCookies(['jwt_token']);


    return {
        // Login
        login: async (email, password) => {
            const token = await apiCall('Identity/token', {
                method: 'POST',
                body: JSON.stringify({
                    userEmail: email,
                    password: password
                }),
                responseType: 'text'
            });
            storeToken(setCookie, token);
            return token;
        },

        // Créer un nouvel utilisateur
        createUser: async (pseudo, email, password) => {
            return apiCall('User/', {
                method: 'POST',
                body: JSON.stringify({
                    pseudo: pseudo,
                    email: email,
                    password: password
                })
            });
        },

        getUserInfo: async (userId) => {
            return apiCall(`User/${userId}`, { method: 'GET' });
        },

        // Dashboard
        getRecentGames: async (userId, count = 4, includeProgression = false, l = "french") => {
            return apiCall(`User/${userId}/recent-games/?count=${count}&includeProgression=${includeProgression}&l=${l}`,{method:'GET'});
        },

        getRecentAchievements: async (userId, count = 4, withPercent, l = "french") => {
            return apiCall(`User/${userId}/recent-achievements/?count=${count}&includeRarity=${withPercent}&l=${l}`,{method:'GET'});

        },
        
        getProgressionGame: async (href, l) => {
            return apiCall(`${href}/?l=${l}`, {method:'GET'});
        },

        // Settings
        changePseudo: async (userId, pseudo) => {
            return apiCall(`User/${userId}`, {
                method: 'PATCH',
                body: JSON.stringify({ pseudo })
            });
        },

        changeEmail: async (userId, email) => {
            return apiCall(`User/${userId}`, {
                method: 'PATCH',
                body: JSON.stringify({ email })
            });
        },

        changePassword: async (userId, oldPassword, newPassword) => {
            return apiCall(`User/${userId}`, {
                method: 'PATCH',
                body: JSON.stringify({
                    oldPassword,
                    newPassword
                })
            });
        }
    };
};