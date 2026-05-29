import { useState, useEffect } from 'react';
import GameDetail from '../../components/GameDetail';
import './index.css';
import { useParams, useSearchParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import {useUserService} from "../../api/userService.js";

const placeholder = {
    title: "UnSuperJeu",
    coverUrl: "/covers/placeholder.png",
    bgUrl: "/covers/placeholderBackground.jpg",
    category: "FriendSlop",
    version: "V 0.Trust WIP",
    year: 2077,
    size: "420.69 MB",
    description: "The DarkSouls of its genra, as no journalist has completed the tutorial yet.\nThe GOTY of this year. Better than everything Riot Games did since the last decade.\nSurprisingly, Capcom did not release it, yet they wish they did."
};

const languageMap = {
    fr: 'french',
    en: 'english',
};


export default function Game() {
    const { i18n , t } = useTranslation();
    const currentLangCode = (i18n.language).substring(0, 2);

    const { getGameInfo } = useUserService();
    const { gameId } = useParams();
    const [searchParams] = useSearchParams();
    
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
    }, [gameId, apiLanguage]); // Le useEffect se relancera si l'ID ou la langue change

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
            <div className="scard glass">
                <GameDetail game={game || placeholder} className="GameDetail"/>
            </div> 
        </div>
    );
}