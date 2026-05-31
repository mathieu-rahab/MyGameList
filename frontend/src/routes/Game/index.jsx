import { useState, useEffect } from 'react';
import GameDetail from '../../components/GameDetail';
import './index.css';
import { useParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import {useUserService} from "../../api/userService.js";


const languageMap = {
    fr: 'french',
    en: 'english',
};


export default function Game() {
    const { i18n , t } = useTranslation();
    const currentLangCode = (i18n.language).substring(0, 2);

    const { getGameInfo } = useUserService();
    const { gameId } = useParams();

    const apiLanguage = languageMap[currentLangCode] || 'french';

    const [game, setGame] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);

    // 2. Utilisation de useEffect pour appeler l'API au chargement du composant
    useEffect(() => {
        // On définit une fonction asynchrone à l'intérieur du useEffect
        const fetchGameData = async () => {
            setIsLoading(true); // On commence le chargement
            try {
                // On attend la réponse de l'API
                const data = await getGameInfo(gameId, apiLanguage);
                setGame(data);
            } catch (err) {
                console.error("Erreur lors de la récupération du jeu :", err);
                setError(t('Game.GameRetrieveError'));
            } finally {
                setIsLoading(false);
            }
        };

        fetchGameData();
        // Le useEffect se relancera si l'ID ou la langue change
    }, [gameId, apiLanguage]); // eslint-disable-line react-hooks/exhaustive-deps

    // 3. Adapter l'affichage en fonction de l'état (Chargement, Erreur, ou Succès)
    if (isLoading) {
        return (
            <div className='main'>
                <p>{t("Game.LoadingGame")}</p>
            </div>
        );
    }

    if (error) {
        return (
            <div className='main'>
                <p className="error-message">{error}</p>
            </div>
        );
    }

    return (
        <div className='main'>
            <GameDetail game={game} className="GameDetail"/>
        </div>
    );
}