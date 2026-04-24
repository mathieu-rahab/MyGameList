import './index.css';
import GamePreview from '../GamePreview';

export default function GameDetail({ game }) {
  return (
    <div className='GameDetail'>
        <GamePreview game={game} className='GamePreview'/>
        <div className='GameDescription'>
            <h2>About the game</h2>
            <span>{game.description}</span>
        </div>


    </div>
  );
}