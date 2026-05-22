import { useApiCall } from './apiClient';

export const useUserService = () => {
    const apiCall = useApiCall();

    return {
        // Login
        login: async (email, password) => {
            return apiCall('Identity/token', {
                method: 'POST',
                body: JSON.stringify({
                    userEmail: email,
                    password: password
                }),
            });
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
        }
    };
};