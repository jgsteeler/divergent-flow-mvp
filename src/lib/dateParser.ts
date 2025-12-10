export function parseNaturalDate(text: string): number | null {
  const now = new Date()
  const today = new Date(now.getFullYear(), now.getMonth(), now.getDate())
  
  const lowerText = text.toLowerCase().trim()
  
  if (lowerText === 'today') {
    return today.getTime()
  }
  
  if (lowerText === 'tomorrow') {
    const tomorrow = new Date(today)
    tomorrow.setDate(tomorrow.getDate() + 1)
    return tomorrow.getTime()
  }
  
  if (lowerText === 'yesterday') {
    const yesterday = new Date(today)
    yesterday.setDate(yesterday.getDate() - 1)
    return yesterday.getTime()
  }
  
  const inDaysMatch = lowerText.match(/^in (\d+) days?$/)
  if (inDaysMatch) {
    const days = parseInt(inDaysMatch[1], 10)
    const future = new Date(today)
    future.setDate(future.getDate() + days)
    return future.getTime()
  }
  
  const inWeeksMatch = lowerText.match(/^in (\d+) weeks?$/)
  if (inWeeksMatch) {
    const weeks = parseInt(inWeeksMatch[1], 10)
    const future = new Date(today)
    future.setDate(future.getDate() + (weeks * 7))
    return future.getTime()
  }
  
  const daysOfWeek = ['sunday', 'monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday']
  
  const nextDayMatch = lowerText.match(/^next (sunday|monday|tuesday|wednesday|thursday|friday|saturday)$/)
  if (nextDayMatch) {
    const targetDay = daysOfWeek.indexOf(nextDayMatch[1])
    const currentDay = now.getDay()
    let daysUntil = targetDay - currentDay
    if (daysUntil <= 0) {
      daysUntil += 7
    }
    const future = new Date(today)
    future.setDate(future.getDate() + daysUntil)
    return future.getTime()
  }
  
  const thisDayMatch = lowerText.match(/^(this )?(sunday|monday|tuesday|wednesday|thursday|friday|saturday)$/)
  if (thisDayMatch) {
    const targetDay = daysOfWeek.indexOf(thisDayMatch[2])
    const currentDay = now.getDay()
    let daysUntil = targetDay - currentDay
    if (daysUntil < 0) {
      daysUntil += 7
    }
    if (daysUntil === 0) {
      return today.getTime()
    }
    const future = new Date(today)
    future.setDate(future.getDate() + daysUntil)
    return future.getTime()
  }
  
  if (lowerText === 'next week') {
    const future = new Date(today)
    future.setDate(future.getDate() + 7)
    return future.getTime()
  }
  
  if (lowerText === 'next month') {
    const future = new Date(today)
    future.setMonth(future.getMonth() + 1)
    return future.getTime()
  }
  
  const monthNames = ['january', 'february', 'march', 'april', 'may', 'june', 
                      'july', 'august', 'september', 'october', 'november', 'december']
  
  const monthDayMatch = lowerText.match(/^(january|february|march|april|may|june|july|august|september|october|november|december) (\d+)(st|nd|rd|th)?$/)
  if (monthDayMatch) {
    const month = monthNames.indexOf(monthDayMatch[1])
    const day = parseInt(monthDayMatch[2], 10)
    let year = now.getFullYear()
    
    const targetDate = new Date(year, month, day)
    if (targetDate < now) {
      year++
      targetDate.setFullYear(year)
    }
    
    return targetDate.getTime()
  }
  
  const numericDateMatch = lowerText.match(/^(\d{1,2})\/(\d{1,2})(\/(\d{2,4}))?$/)
  if (numericDateMatch) {
    const month = parseInt(numericDateMatch[1], 10) - 1
    const day = parseInt(numericDateMatch[2], 10)
    let year = numericDateMatch[4] 
      ? (numericDateMatch[4].length === 2 
          ? 2000 + parseInt(numericDateMatch[4], 10) 
          : parseInt(numericDateMatch[4], 10))
      : now.getFullYear()
    
    const targetDate = new Date(year, month, day)
    if (!numericDateMatch[4] && targetDate < now) {
      year++
      targetDate.setFullYear(year)
    }
    
    return targetDate.getTime()
  }
  
  const endOfWeekMatch = lowerText.match(/^(end of|eow)$/)
  if (endOfWeekMatch) {
    const daysUntilFriday = (5 - now.getDay() + 7) % 7 || 7
    const future = new Date(today)
    future.setDate(future.getDate() + daysUntilFriday)
    return future.getTime()
  }
  
  const endOfMonthMatch = lowerText.match(/^(end of month|eom)$/)
  if (endOfMonthMatch) {
    const lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0)
    return lastDay.getTime()
  }
  
  return null
}

export function extractDateFromText(text: string): { date: number | null; cleanText: string } {
  const patterns = [
    /\b(today|tomorrow|yesterday)\b/i,
    /\bin (\d+) (days?|weeks?)\b/i,
    /\bnext (sunday|monday|tuesday|wednesday|thursday|friday|saturday|week|month)\b/i,
    /\b(this )?(sunday|monday|tuesday|wednesday|thursday|friday|saturday)\b/i,
    /\b(january|february|march|april|may|june|july|august|september|october|november|december) (\d+)(st|nd|rd|th)?\b/i,
    /\b(\d{1,2})\/(\d{1,2})(\/(\d{2,4}))?\b/,
    /\b(end of week|eow|end of month|eom)\b/i,
  ]
  
  for (const pattern of patterns) {
    const match = text.match(pattern)
    if (match) {
      const dateString = match[0]
      const parsedDate = parseNaturalDate(dateString)
      
      if (parsedDate !== null) {
        const cleanText = text.replace(pattern, '').replace(/\s+/g, ' ').trim()
        return { date: parsedDate, cleanText }
      }
    }
  }
  
  return { date: null, cleanText: text }
}

export function formatDate(timestamp: number): string {
  const date = new Date(timestamp)
  const today = new Date()
  const tomorrow = new Date(today)
  tomorrow.setDate(tomorrow.getDate() + 1)
  
  today.setHours(0, 0, 0, 0)
  tomorrow.setHours(0, 0, 0, 0)
  date.setHours(0, 0, 0, 0)
  
  if (date.getTime() === today.getTime()) {
    return 'Today'
  }
  
  if (date.getTime() === tomorrow.getTime()) {
    return 'Tomorrow'
  }
  
  const diffDays = Math.round((date.getTime() - today.getTime()) / (1000 * 60 * 60 * 24))
  
  if (diffDays > 0 && diffDays <= 7) {
    return `in ${diffDays} day${diffDays > 1 ? 's' : ''}`
  }
  
  const options: Intl.DateTimeFormatOptions = { month: 'short', day: 'numeric' }
  if (date.getFullYear() !== today.getFullYear()) {
    options.year = 'numeric'
  }
  
  return date.toLocaleDateString('en-US', options)
}
