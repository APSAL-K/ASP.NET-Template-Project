import { ChevronLeft, ChevronRight } from 'lucide-react'

type PaginationProps = {
  currentPage: number
  totalPages: number
  totalItems: number
  onPageChange: (page: number) => void
  pageSize: number
}

export function Pagination({ 
  currentPage, 
  totalPages, 
  totalItems, 
  onPageChange,
  pageSize 
}: PaginationProps) {
  if (totalPages <= 1) return null

  const startIdx = (currentPage - 1) * pageSize + 1
  const endIdx = Math.min(currentPage * pageSize, totalItems)

  return (
    <div className="pagination-container">
      <div className="pagination-info">
        Showing <span className="font-bold">{startIdx}-{endIdx}</span> of <span className="font-bold">{totalItems}</span> results
      </div>
      
      <div className="pagination-controls">
        <button 
          className="pagination-btn" 
          disabled={currentPage === 1}
          onClick={() => onPageChange(currentPage - 1)}
        >
          <ChevronLeft size={16} />
        </button>
        
        <div className="pagination-pages">
          {[...Array(totalPages)].map((_, i) => {
            const pageNum = i + 1
            const isCurrent = pageNum === currentPage
            
            // Basic elipsis logic could go here, but for now simple page list
            return (
              <button
                key={pageNum}
                className={`pagination-page-btn ${isCurrent ? 'active' : ''}`}
                onClick={() => onPageChange(pageNum)}
              >
                {pageNum}
              </button>
            )
          })}
        </div>

        <button 
          className="pagination-btn" 
          disabled={currentPage === totalPages}
          onClick={() => onPageChange(currentPage + 1)}
        >
          <ChevronRight size={16} />
        </button>
      </div>
    </div>
  )
}
