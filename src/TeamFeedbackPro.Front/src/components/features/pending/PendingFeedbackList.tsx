import type { FeedbackResult } from '../../../types';
import { FeedbackPendingCard } from './PendingFeedbackCard';
import './css/PendingFeedbackList.css';

type FeedbackListProps = {
  feedbacks: FeedbackResult[];
  onApprove: (item: FeedbackResult) => void;
  onReject: (item: FeedbackResult) => void;
};

export const FeedbackPendingList = ({ feedbacks, onApprove, onReject }: FeedbackListProps) => {

  if (feedbacks.length === 0) {
    return (
      <div className="feedback-list-empty">
        <p>Nenhum feedback pendente para mostrar.</p>
      </div>
    );
  }

  return (
    <section className="feedback-list">
      {feedbacks.map((fb) => (
        <FeedbackPendingCard
          key={fb.id}
          feedback={fb}
          onApproveRequest={onApprove}
          onRejectRequest={onReject}
        />
      ))}
    </section>
  );
};