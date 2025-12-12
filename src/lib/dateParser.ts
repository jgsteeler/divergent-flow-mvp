export function parseTimeOfDay(text: string): { hours: number; minutes: number } | null {
  const lowerText = text.toLowerCase().trim()
  
  if (lowerText === 'noon' || lowerText === 'midday') {
    return { hours: 12, minutes: 0 }
  }
  
  if (lowerText === 'midnight') {
    return { hours: 0, minutes: 0 }
  }
  
  if (lowerText === 'morning') {
    return { hours: 9, minutes: 0 }
  }
  
  if (lowerText === 'afternoon') {
    return { hours: 14, minutes: 0 }
  }
  
  if (lowerText === 'evening') {
    return { hours: 18, minutes: 0 }
  }
  
  if (lowerText === 'night') {
    return { hours: 20, minutes: 0 }
  }
  
  const time12Match = lowerText.match(/^(\d{1,2})(?::(\d{2}))?\s*(am|pm)$/)
  if (time12Match) {
    let hours = parseInt(time12Match[1], 10)
    const minutes = time12Match[2] ? parseInt(time12Match[2], 10) : 0
    const meridiem = time12Match[3]
    
    if (hours < 1 || hours > 12 || minutes < 0 || minutes > 59) {
      return null
    }
    
    if (meridiem === 'pm' && hours !== 12) {
      hours += 12
    } else if (meridiem === 'am' && hours === 12) {
      hours = 0
    }
    
    return { hours, minutes }
  }
  
  const time24Match = lowerText.match(/^(\d{1,2}):(\d{2})$/)
  if (time24Match) {
    const hours = parseInt(time24Match[1], 10)
    const minutes = parseInt(time24Match[2], 10)
    
    if (hours < 0 || hours > 23 || minutes < 0 || minutes > 59) {
      return null
    }
    
    return { hours, minutes }
  }
  
  return null
}

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

export function extractTimeFromText(text: string): { time: { hours: number; minutes: number } | null; cleanText: string } {
  const timePatterns = [
    /\b(\d{1,2}(?::\d{2})?\s*(?:am|pm))\b/i,
    /\b(\d{1,2}:\d{2})\b/,
    /\b(at )?(noon|midday|midnight|morning|afternoon|evening|night)\b/i,
  ]
  
  for (const pattern of timePatterns) {
    const match = text.match(pattern)
    if (match) {
      const timeString = match[match.length - 1]
      const parsedTime = parseTimeOfDay(timeString)
      
      if (parsedTime !== null) {
        const cleanText = text.replace(match[0], '').replace(/\s+/g, ' ').trim()
        return { time: parsedTime, cleanText }
      }
    }
  }
  
  return { time: null, cleanText: text }
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

export function extractDateTimeFromText(text: string): { dateTime: number | null; cleanText: string } {
  const { date, cleanText: textAfterDate } = extractDateFromText(text)
  const { time, cleanText: textAfterTime } = extractTimeFromText(textAfterDate)
  
  if (date !== null && time !== null) {
    const dateObj = new Date(date)
    dateObj.setHours(time.hours, time.minutes, 0, 0)
    return { dateTime: dateObj.getTime(), cleanText: textAfterTime }
  }
  
  if (date !== null) {
    return { dateTime: date, cleanText: textAfterDate }
  }
  
  if (time !== null) {
    const now = new Date()
    const dateObj = new Date(now.getFullYear(), now.getMonth(), now.getDate(), time.hours, time.minutes, 0, 0)
    
    if (dateObj < now) {
      dateObj.setDate(dateObj.getDate() + 1)
    }
    
    return { dateTime: dateObj.getTime(), cleanText: textAfterTime }
  }
  
  return { dateTime: null, cleanText: text }
}

export function formatDate(timestamp: number): string {
  const date = new Date(timestamp)
  const today = new Date()
  const tomorrow = new Date(today)
  tomorrow.setDate(tomorrow.getDate() + 1)
  
  const dateOnly = new Date(date)
  const todayOnly = new Date(today)
  const tomorrowOnly = new Date(tomorrow)
  
  todayOnly.setHours(0, 0, 0, 0)
  tomorrowOnly.setHours(0, 0, 0, 0)
  dateOnly.setHours(0, 0, 0, 0)
  
  const hasTime = date.getHours() !== 0 || date.getMinutes() !== 0
  
  let dateStr = ''
  
  if (dateOnly.getTime() === todayOnly.getTime()) {
    dateStr = 'Today'
  } else if (dateOnly.getTime() === tomorrowOnly.getTime()) {
    dateStr = 'Tomorrow'
  } else {
    const diffDays = Math.round((dateOnly.getTime() - todayOnly.getTime()) / (1000 * 60 * 60 * 24))
    
    if (diffDays > 0 && diffDays <= 7) {
      dateStr = `in ${diffDays} day${diffDays > 1 ? 's' : ''}`
    } else {
      const options: Intl.DateTimeFormatOptions = { month: 'short', day: 'numeric' }
      if (date.getFullYear() !== today.getFullYear()) {
        options.year = 'numeric'
      }
      dateStr = date.toLocaleDateString('en-US', options)
    }
  }
  
  if (hasTime) {
    const timeStr = date.toLocaleTimeString('en-US', { 
      hour: 'numeric', 
      minute: '2-digit',
      hour12: true 
    })
    return `${dateStr} at ${timeStr}`
  }
  
  return dateStr
}
