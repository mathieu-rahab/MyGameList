import { useCookies } from 'react-cookie';
import {shouldRenew, storeToken} from '../utils/tokenUtils.js';

const API_BASE = 'http://localhost:5131/api/';

export const useApiCall = () => {
    const [cookies, setCookie] = useCookies(['jwt_token']);

    // Pour renouveler le token
    const rawFetch = async (endpoint, options = {}) => {
        const { responseType = 'json', ...fetchOptions } = options;

        const response = await fetch(
            endpoint.startsWith('http') ? endpoint : `${API_BASE}${endpoint}`,
            { ...fetchOptions, headers: { 'Content-Type': 'application/json', ...fetchOptions.headers } }
        );

        if (!response.ok) {
            const err = await response.json().catch(() => ({}));
            return Promise.reject({ status: response.status, error: err?.error });
        }

        return responseType === 'text' ? response.text() : response.json();
    };

    // Fonction principal
    return async (endpoint, options = {}) => {
        let token = cookies.jwt_token;

        if (token && shouldRenew(token)) {
            try {
                const newToken = await rawFetch('Identity/renew', {
                    method: 'POST',
                    responseType: 'text',
                    headers: { Authorization: `Bearer ${token}` },
                });
                storeToken(setCookie, newToken);
                token = newToken;
            } catch {
                // on continue avec le token actuel
            }
        }

        return rawFetch(endpoint, {
            ...options,
            headers: {
                ...options.headers,
                ...(token && { Authorization: `Bearer ${token}` }),
            },
        });
    };
};