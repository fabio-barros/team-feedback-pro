import React from 'react';


type TabsProps = {
  tabs: string[]; 
  activeTab: string;
  onTabChange: (tabLabel: string) => void;
};

export const Tabs = ({ tabs, activeTab, onTabChange }: TabsProps) => {
  return (
    <nav className="tabs-container">
      {tabs.map((label) => (
        <button
          key={label}
          className={`tab-button ${activeTab === label ? 'tab-button--active' : ''}`}
          onClick={() => onTabChange(label)}
        >
          {label}
        </button>
      ))}
    </nav>
  );
};