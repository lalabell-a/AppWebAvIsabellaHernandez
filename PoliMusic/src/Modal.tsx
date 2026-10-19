import { useRef, useEffect, type ReactNode } from 'react';
import { X } from 'lucide-react';
export default function Modal({ children, title, onClose }: {
    children: ReactNode;
    title: string;
    onClose: () => void;
}) { const ref = useRef<HTMLDialogElement>(null); useEffect(() => { ref.current?.showModal(); }, []); return <dialog ref={ref} onCancel={onClose} onClick={e => { if (e.target === ref.current)
    onClose(); }}><div className="modal-head"><h2>{title}</h2><button aria-label="Cerrar" onClick={onClose}><X /></button></div>{children}</dialog>; }
