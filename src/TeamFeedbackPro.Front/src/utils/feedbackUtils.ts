// src/utils/feedbackUtils.ts

export const getEmojiForFeeling = (text?: string) => {
  if (!text) return '💭'; 
  
  const t = text.toLowerCase();
  
  if (t.includes('feliz') || t.includes('happy')) return '😄';
  if (t.includes('triste') || t.includes('sad')) return '😔';
  if (t.includes('surpreso') || t.includes('surprised')) return '😮';
  if (t.includes('raiva') || t.includes('angry')) return '😡';
  if (t.includes('pensativo') || t.includes('thoughtful')) return '🤔';
  
  return '😐'; // Caso não encontre
};


export const getFeelingColorClass = (text?: string) => {
  if (!text) return '';
  const t = text.toLowerCase();
  if (t.includes('raiva') || t.includes('triste')) return 'feeling-negative';
  if (t.includes('feliz') || t.includes('surpreso')) return 'feeling-positive';
  return 'feeling-neutral';
};