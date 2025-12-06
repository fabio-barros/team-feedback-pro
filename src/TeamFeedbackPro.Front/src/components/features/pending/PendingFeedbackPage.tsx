import { useEffect, useState } from "react";
import { FeedbackPendingList } from "./PendingFeedbackList";
import { ModalConfirm } from "../../ui/ModalConfirm";
import { Toast } from "../../ui/Toast";

import type { FeedbackResult } from "../../../types";
import { getPendingFeedbacks, approveFeedback, rejectFeedback } from "../../../services/feedbackService";

export const FeedbackPendingPage = () => {

  const [feedbacks, setFeedbacks] = useState<FeedbackResult[]>([]);
  const [selected, setSelected] = useState<FeedbackResult | null>(null);
  const [modalAction, setModalAction] = useState<"approve" | "reject" | null>(null);
  const [rejectionText, setRejectionText] = useState(""); 

  const [toast, setToast] = useState<{ msg: string; type: "success"|"error" } | null>(null);

  const carregar = async () => {
  const dados = await getPendingFeedbacks();
  setFeedbacks(dados.items);
};

  useEffect(() => {
    carregar();
  }, []);

  const openApproveModal = (item: FeedbackResult) => {
    setSelected(item);
    setModalAction("approve");
    setRejectionText("");
  };

  const openRejectModal = (item: FeedbackResult) => {
    setSelected(item);
    setModalAction("reject");
    setRejectionText("");
  };

  const closeModal = () => {
    setSelected(null);
    setModalAction(null);
    setRejectionText("");
  };

  const executeAction = async () => {
    if (!selected || !modalAction) return;

    if (modalAction === "reject" && rejectionText.trim().length === 0) {
      setToast({ msg: "Informe a justificativa da rejeição.", type: "error" });
      return;
    }

    if (modalAction === "reject" && rejectionText.trim().length < 20) {
      setToast({ msg: "A justificativa deve ter pelo menos 10 caracteres.", type: "error" });
      return;
    }

    try {
      if (modalAction === "approve") {
        await approveFeedback(selected.id, "Feedback aprovado pelo gestor");
        setToast({ msg: "Feedback aprovado com sucesso!", type: "success" });
      } else {
        await rejectFeedback(selected.id, rejectionText);
        setToast({ msg: "Feedback rejeitado.", type: "error" });
      }

      // remover da lista
      setFeedbacks(prev => prev.filter(f => f.id !== selected.id));

    } catch (error) {
      setToast({ msg: "Erro ao atualizar feedback.", type: "error" });
    }

    closeModal();
  };

  return (
    <>
      <FeedbackPendingList 
        feedbacks={feedbacks}
        onApprove={openApproveModal}
        onReject={openRejectModal}
      />

      <ModalConfirm
        open={!!selected}
        title={modalAction === "approve" ? "Aprovar Feedback" : "Rejeitar Feedback"}
        message={`Tem certeza que deseja ${modalAction === "approve" ? "aprovar" : "rejeitar"} este feedback?`}
        confirmLabel={modalAction === "approve" ? "Aprovar" : "Rejeitar"}
        textareaEnabled={modalAction === "reject"}
        textareaValue={rejectionText}
        onTextareaChange={setRejectionText}
        onConfirm={executeAction}
        onCancel={closeModal}
      />

      {toast && (
        <Toast 
          message={toast.msg}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}
    </>
  );
};
