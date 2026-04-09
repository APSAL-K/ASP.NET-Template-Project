import { useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../app/hooks'
import { dismissToast } from '../features/ui/uiSlice'

export function ToastViewport() {
  const dispatch = useAppDispatch()
  const toasts = useAppSelector((state) => state.ui.toasts)

  useEffect(() => {
    const timers = toasts.map((toast) =>
      window.setTimeout(() => {
        dispatch(dismissToast(toast.id))
      }, 4200),
    )

    return () => {
      timers.forEach((timer) => window.clearTimeout(timer))
    }
  }, [dispatch, toasts])

  return (
    <aside className="toast-viewport" aria-live="polite" aria-atomic="true">
      {toasts.map((toast) => (
        <article key={toast.id} className={`toast-card toast-card--${toast.kind}`}>
          <div>
            <strong>{toast.title}</strong>
            {toast.description ? <p>{toast.description}</p> : null}
          </div>
          <button type="button" onClick={() => dispatch(dismissToast(toast.id))} aria-label="Dismiss toast">
            x
          </button>
        </article>
      ))}
    </aside>
  )
}
