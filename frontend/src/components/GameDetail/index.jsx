import './index.css';
import GamePreview from '../GamePreview';
import { useTranslation } from "react-i18next";

export default function GameDetail({game}) {
  //traduction
  const {t} = useTranslation();

  return (
      <div className='game-detail-container'>
          <GamePreview game={game} className='GamePreview'/>
          <div className="scard glass game-description">
              <h2>{t('GameDetail.AboutTheGame')}</h2>
              <hr className="line"/>
              <span dangerouslySetInnerHTML={{__html:game.description}} className="description"/>
          </div>
      </div>
  );
}