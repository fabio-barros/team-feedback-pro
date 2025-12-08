import { useEffect, useState } from 'react';
import { getDashboardData } from '../../../services/sprintService';
import type { DashboardData, UserProfile } from '../../../types';
import './css/DashboarPage.css';
import { getTeamById } from '../../../services/teamService';

const EMOTION_COLORS = {
  feliz: '#48BB78',     
  pensativo: '#A0AEC0', 
  surpreso: '#ED8936',  
  triste: '#4299E1',    
  raiva: '#F56565'      
};

const EMOTION_EMOJIS = {
  feliz: '😄',
  pensativo: '🤔',
  surpreso: '😲',
  triste: '😢',
  raiva: '😡'
};

type Props = {
  user: UserProfile | null;
};

export const DashboardPage = ({ user }: Props) => {
  const [data, setData] = useState<DashboardData[]>([]);
  const [teamName, setTeamName] = useState('');

  useEffect(() => {
    getDashboardData().then(setData);

    if (user?.teamId) {
      getTeamById(user.teamId)
        .then(t => setTeamName(t.name))
        .catch(() => setTeamName(''));
    }
  }, [user]);
 

  return (
    <div className="dashboard-container">
      <h3>Termômetro da Equipe {teamName ? `- ${teamName}` : ''} por Sprint</h3>
      
      <div className="sprints-grid">
        {data.map((sprint) => (
          <div key={sprint.sprintId} className="sprint-card">
            <h4>{sprint.sprintName}</h4>
            <div className="total-badge">{sprint.totalFeedbacks} feedbacks</div>
            
            <div className="chart-bars">
              {Object.entries(sprint.emotions).map(([emotion, count]) => {
                // @ts-ignore
                const percent = sprint.totalFeedbacks > 0 ? (count / sprint.totalFeedbacks) * 100 : 0;
                
                return (
                  <div key={emotion} className="bar-row">
                    <span className="bar-label" title={emotion}>
                      {/* @ts-ignore */}
                      {EMOTION_EMOJIS[emotion]} 
                    </span>
                    <div className="bar-track">
                      <div 
                        className="bar-fill" 
                        style={{ 
                          width: `${percent}%`, 
                          // @ts-ignore
                          backgroundColor: EMOTION_COLORS[emotion] 
                        }}
                      />
                    </div>
                    <span className="bar-count">{count}</span>
                  </div>
                );
              })}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};