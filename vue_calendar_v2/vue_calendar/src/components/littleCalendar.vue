<template>
  <div class="calendar-sidebar">
    <div class="calendar-header">
      <button @click="goToPreviousMonth" class="nav-button">◀</button>
      <h3>{{ currentMonthYear }}</h3>
      <button @click="goToNextMonth" class="nav-button">▶</button>
    </div>
    <div class="calendar-grid">
      <div
          v-for="day in completeCalendarDays"
          :key="day.date?.toISOString() || day.day"
          class="calendar-day"
          :class="{
          'current-month': day.isCurrentMonth,
          'other-month': !day.isCurrentMonth,
          'today': day.isToday
        }"
      >
        {{ day.day }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const currentDate = ref(new Date())

const completeCalendarDays = computed(() => {
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()

  // První den měsíce
  const firstDay = new Date(year, month, 1)

  // Poslední den měsíce
  const lastDay = new Date(year, month + 1, 0)

  const days = []

  // Přidání dnů před začátkem měsíce (předchozí měsíc)
  const startingDay = firstDay.getDay()
  const prevMonthLastDay = new Date(year, month, 0)
  for (let i = 0; i < startingDay; i++) {
    const prevMonthDay = prevMonthLastDay.getDate() - startingDay + i + 1
    days.push({
      day: prevMonthDay,
      date: new Date(year, month - 1, prevMonthDay),
      isCurrentMonth: false
    })
  }

  // Přidání dnů aktuálního měsíce
  for (let day = 1; day <= lastDay.getDate(); day++) {
    const currentDay = new Date(year, month, day)
    days.push({
      day,
      date: currentDay,
      isCurrentMonth: true,
      isToday: isSameDay(currentDay, new Date())
    })
  }

  // Přidání dnů následujícího měsíce
  const remainingSlots = 42 - days.length // Celkem 6 řádků po 7 dnech
  for (let day = 1; day <= remainingSlots; day++) {
    days.push({
      day,
      date: new Date(year, month + 1, day),
      isCurrentMonth: false
    })
  }

  return days
})

const currentMonthYear = computed(() => {
  return currentDate.value.toLocaleString('cs-CZ', {
    month: 'long',
    year: 'numeric'
  })
})

function goToPreviousMonth() {
  currentDate.value = new Date(
      currentDate.value.getFullYear(),
      currentDate.value.getMonth() - 1,
      1
  )
}

function goToNextMonth() {
  currentDate.value = new Date(
      currentDate.value.getFullYear(),
      currentDate.value.getMonth() + 1,
      1
  )
}

function isSameDay(date1, date2) {
  return (
      date1.getFullYear() === date2.getFullYear() &&
      date1.getMonth() === date2.getMonth() &&
      date1.getDate() === date2.getDate()
  )
}
</script>

<style scoped>
.calendar-header {
  display: flex;
  justify-content: space-between;
  gap: 15px;
  align-items: center;
  margin-bottom: 5px;
  margin-right: 2px;
  font-size: 14px;
}

.nav-button {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 14px;
  color: white;
}

.calendar-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 1px;
  text-align: center;
}

.calendar-day {
  padding: 3px 5px 3px 5px;
  font-size: 12px;
}

.current-month {
  color: white;
}

.other-month {
  color: #aaa;
}

.today {
  background-color: cornflowerblue;
  color: white;
  font-weight: bold;
  border-radius: 100%;
}
</style>