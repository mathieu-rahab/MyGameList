import { useCookies } from 'react-cookie';

const API_BASE = '/api';

// Hook pour récupérer le token
export const useApiCall = () => {
    const [cookies] = useCookies(['jwt_token']);

    return async (endpoint, options = {}) => {
        const token = cookies.jwt_token;

        const headers = {
            'Content-Type': 'application/json',
            ...options.headers,
        };

        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }

        try {
            const response = await fetch(`${API_BASE}${endpoint}`, {
                ...options,
                headers,
            });

            if (!response.ok) {
                throw new Error(`${response.status}`);
            }

            return await response.json();
        } catch (error) {
            console.error('Erreur API:', error);
            throw error;
        }
    };
};