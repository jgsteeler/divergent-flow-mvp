import { Button } from '@/components/ui/button'
import { Card } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { ArrowLeft, CheckCircle, Clock, Fire } from '@phosphor-icons/react'
import { motion } from 'framer-motion'
import { useDashboard } from '@/hooks/useItems'
import { TaskItem } from '@/lib/api/itemsApi'

interface DashboardProps {
  onBack: () => void
}

export function Dashboard({ onBack }: DashboardProps) {
  const { data, isLoading, error } = useDashboard()

  const formatDate = (timestamp: number) => {
    const date = new Date(timestamp)
    const today = new Date()
    const yesterday = new Date(today)
    yesterday.setDate(yesterday.getDate() - 1)
    
    if (date.toDateString() === today.toDateString()) {
      return 'Today'
    } else if (date.toDateString() === yesterday.toDateString()) {
      return 'Yesterday'
    } else {
      return date.toLocaleDateString()
    }
  }

  const TaskCard = ({ task }: { task: TaskItem }) => (
    <motion.div
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className="p-4 border rounded-lg hover:bg-accent/5 transition-colors"
    >
      <div className="flex items-start justify-between gap-3">
        <p className="text-sm flex-1">{task.text}</p>
        {task.inferredType && (
          <Badge variant="outline" className="shrink-0 text-xs">
            {task.inferredType}
          </Badge>
        )}
      </div>
      <time className="text-xs text-muted-foreground mt-2 block">
        {formatDate(task.createdAt)}
      </time>
    </motion.div>
  )

  return (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      className="w-full space-y-6"
    >
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold">Dashboard</h2>
          <p className="text-sm text-muted-foreground mt-1">
            Your task overview
          </p>
        </div>
        <Button
          onClick={onBack}
          variant="outline"
          className="flex items-center gap-2"
        >
          <ArrowLeft />
          Back
        </Button>
      </div>

      {isLoading && (
        <div className="text-center py-12 text-muted-foreground">
          <p>Loading dashboard...</p>
        </div>
      )}

      {error && (
        <div className="text-center py-12 text-destructive">
          <p>Failed to load dashboard: {error.message}</p>
        </div>
      )}

      {!isLoading && !error && data && (
        <>
          {/* Metrics Cards */}
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <Card className="p-4">
              <div className="text-2xl font-bold">{data.metrics.totalItems}</div>
              <div className="text-xs text-muted-foreground mt-1">Total Items</div>
            </Card>
            <Card className="p-4">
              <div className="text-2xl font-bold">{data.metrics.pendingReview}</div>
              <div className="text-xs text-muted-foreground mt-1">Pending Review</div>
            </Card>
            <Card className="p-4">
              <div className="text-2xl font-bold">{data.metrics.actionItems}</div>
              <div className="text-xs text-muted-foreground mt-1">Action Items</div>
            </Card>
            <Card className="p-4">
              <div className="text-2xl font-bold">{data.metrics.completedToday}</div>
              <div className="text-xs text-muted-foreground mt-1">Completed Today</div>
            </Card>
          </div>

          {/* Task Lists */}
          <div className="grid md:grid-cols-3 gap-6">
            {/* Overdue Tasks */}
            <Card className="p-6">
              <div className="flex items-center gap-2 mb-4">
                <Fire className="text-destructive" size={20} />
                <h3 className="font-semibold text-lg">Overdue</h3>
                <Badge variant="destructive" className="ml-auto">
                  {data.overdueTasks.length}
                </Badge>
              </div>
              <div className="space-y-3">
                {data.overdueTasks.length === 0 ? (
                  <p className="text-sm text-muted-foreground text-center py-4">
                    No overdue tasks
                  </p>
                ) : (
                  data.overdueTasks.map((task) => (
                    <TaskCard key={task.id} task={task} />
                  ))
                )}
              </div>
            </Card>

            {/* Today's Tasks */}
            <Card className="p-6">
              <div className="flex items-center gap-2 mb-4">
                <Clock className="text-primary" size={20} />
                <h3 className="font-semibold text-lg">Today</h3>
                <Badge className="ml-auto">
                  {data.todayTasks.length}
                </Badge>
              </div>
              <div className="space-y-3">
                {data.todayTasks.length === 0 ? (
                  <p className="text-sm text-muted-foreground text-center py-4">
                    No tasks for today
                  </p>
                ) : (
                  data.todayTasks.map((task) => (
                    <TaskCard key={task.id} task={task} />
                  ))
                )}
              </div>
            </Card>

            {/* Upcoming Tasks */}
            <Card className="p-6">
              <div className="flex items-center gap-2 mb-4">
                <CheckCircle className="text-muted-foreground" size={20} />
                <h3 className="font-semibold text-lg">Upcoming</h3>
                <Badge variant="outline" className="ml-auto">
                  {data.upcomingTasks.length}
                </Badge>
              </div>
              <div className="space-y-3">
                {data.upcomingTasks.length === 0 ? (
                  <p className="text-sm text-muted-foreground text-center py-4">
                    No upcoming tasks
                  </p>
                ) : (
                  data.upcomingTasks.map((task) => (
                    <TaskCard key={task.id} task={task} />
                  ))
                )}
              </div>
            </Card>
          </div>
        </>
      )}
    </motion.div>
  )
}
