import { useCookies } from 'react-cookie';

const API_BASE = 'http://localhost:5131/api/';

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
            const response = await fetch(endpoint.startsWith('http') ? endpoint : `${API_BASE}${endpoint}`, {
                ...options,
                headers,
            });

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
            return await response.json();
        } catch (error) {
            throw error;
        }
    };
};