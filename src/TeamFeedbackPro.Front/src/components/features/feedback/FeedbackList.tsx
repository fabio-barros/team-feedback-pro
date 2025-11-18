import React from 'react';
import type { Feedback } from '../../../types';
import { FeedbackCard } from './FeedbackCard';
import './css/FeedbackList.css'; 

type FeedbackListProps = {
  feedbacks: Feedback[];
  perspectiva: 'target' | 'author';
};

export const FeedbackList = ({ feedbacks, perspectiva }: FeedbackListProps) => {
  
  if (feedbacks.length === 0) {
    return (
      <div className="feedback-list-empty">
        <p>Nenhum feedback interessante para mostrar aqui.</p>
      </div>
    );
  }

  // --- A Lista ---
  return (
    <section className="feedback-list">
      {feedbacks.map((fb) => (
        <FeedbackCard 
          key={fb.id} 
          feedback={fb} 
          perspectiva={perspectiva} 
        />
      ))}
    </section>
  );
};