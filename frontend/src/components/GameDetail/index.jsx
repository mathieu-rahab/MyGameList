import './index.css';
import GamePreview from '../GamePreview';
import { useTranslation } from "react-i18next";

export default function GameDetail({ game }) {
  //traduction
  const {t} = useTranslation();
  //

  return (
    <div className='GameDetail'>
        <GamePreview game={game} className='GamePreview'/>
        <div className='GameDescription'>
            <h2>{t('GameDetail.AboutTheGame')}</h2>
            <span>{game.description}</span>
        </div>


    </div>
  );
}