import { useApiCall } from './apiClient';

export const useUserService = () => {
    const apiCall = useApiCall();

    return {
        getRecentGames: async (userId, count = 4, includeAchievements = true) => {
            return apiCall(`/User/${userId}/recent-games/?count=${count}&includeAchievements=${includeAchievements}`,{method:'GET'});
        },

        changePseudo: async (userId, pseudo) => {
            return apiCall(`/User/${userId}`, {
                method: 'PATCH',
                body: JSON.stringify({ pseudo })
            });
        },

        changeEmail: async (userId, email) => {
            return apiCall(`/User/${userId}`, {
                method: 'PATCH',
                body: JSON.stringify({ email })
            });
        },

        changePassword: async (userId, oldPassword, newPassword) => {
            return apiCall(`/User/${userId}`, {
                method: 'PATCH',
                body: JSON.stringify({
                    oldPassword,
                    newPassword
                })
            });
        }
    };
};