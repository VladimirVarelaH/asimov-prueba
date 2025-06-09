<script setup>
import { ref, computed, onMounted, watch } from 'vue'

const today = new Date()
const currentMonth = ref(today.getMonth())
const currentYear = ref(today.getFullYear())
const selectedDate = ref(null)
const takenAppointments = ref({})

const daysInMonth = (month, year) => new Date(year, month + 1, 0).getDate()

const firstDayOfMonth = (month, year) => {
  return new Date(year, month, 1).getDay()
}

const adjustedFirstDay = computed(() => {
  const day = firstDayOfMonth(currentMonth.value, currentYear.value)
  return day === 0 ? 6 : day - 1
})

const changeMonth = (delta) => {
  currentMonth.value += delta
  if (currentMonth.value > 11) {
    currentMonth.value = 0
    currentYear.value += 1
  } else if (currentMonth.value < 0) {
    currentMonth.value = 11
    currentYear.value -= 1
  }
}

const emit = defineEmits(['update-hours'])

const selectDate = (day) => {
  const date = new Date(currentYear.value, currentMonth.value, day)
  selectedDate.value = date

  const dateString = date.toISOString().split('T')[0]
  const hours = takenAppointments.value[dateString] || []

  // Convierte las horas tipo "09:00" a número: 9, 10, etc.
  const hourNumbers = hours.map(h => parseInt(h.split(':')[0], 10))

  emit('update-hours', {
    date: dateString,
    hours: hourNumbers
  })
}

const isDayCompleted = (day) => {
  const date = new Date(currentYear.value, currentMonth.value, day)
  const dateString = date.toISOString().split('T')[0]
  const takenHours = takenAppointments.value[dateString] || []
  return takenHours.length >= 10
}



// Función para llamar a la API
const fetchTakenAppointments = async () => {
  try {
    const response = await fetch(`http://localhost:5077/api/appointment/taken?year=${currentYear.value}&month=${currentMonth.value + 1}`)
    if (!response.ok) throw new Error('Error al cargar citas')
    const data = await response.json()
    takenAppointments.value = data
    console.log('Citas bloqueadas:', data)
  } catch (error) {
    console.error(error)
  }
}

const isWeekend = (day) => {
  const date = new Date(currentYear.value, currentMonth.value, day)
  const dayOfWeek = date.getDay()
  return dayOfWeek === 0 || dayOfWeek === 6
}

// Llamar al cargar componente
onMounted(fetchTakenAppointments)

// Llamar cada vez que cambie mes o año
watch([currentMonth, currentYear], fetchTakenAppointments)

defineExpose({
  fetchTakenAppointments
})
</script>


<template>
  <div class="card p-3" style="width: 415px;">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <button class="btn btn-outline-primary btn-sm" @click="changeMonth(-1)">←</button>
      <h5 class="mb-0">
        {{ new Date(currentYear, currentMonth).toLocaleString('default', { month: 'long' }) }}
        {{ currentYear }}
      </h5>
      <button class="btn btn-outline-primary btn-sm" @click="changeMonth(1)">→</button>
    </div>

    <!-- Nombres de los días -->
    <div class="d-flex flex-wrap text-center fw-bold mb-2">
      <div class="m-1" style="width: 45px;">Lu</div>
      <div class="m-1" style="width: 45px;">Ma</div>
      <div class="m-1" style="width: 45px;">Mi</div>
      <div class="m-1" style="width: 45px;">Ju</div>
      <div class="m-1" style="width: 45px;">Vi</div>
      <div class="m-1" style="width: 45px;">Sa</div>
      <div class="m-1" style="width: 45px;">Do</div>
    </div>

    <!-- Calendario -->
    <div class="d-flex flex-wrap">
      <!-- Espacios en blanco antes del primer día -->
      <div v-for="n in adjustedFirstDay" :key="'empty-' + n"
        style="width: 45px; height: 45px;" class="m-1"></div>

        <!-- Días del mes -->
        <div
            v-for="(day) in daysInMonth(currentMonth, currentYear)"
            :key="day"
            class="btn m-1"
            :class="[
                selectedDate?.getDate() === day 
                && selectedDate?.getMonth() === currentMonth 
                && selectedDate?.getFullYear() === currentYear 
                ? 'btn-primary text-white' 
                : 'btn-light',
                isWeekend(day) ? 'weekend' : '',
                isDayCompleted(day) ? 'completed' : ''
            ]"
            style="width: 45px; height: 45px;"
            @click="!isWeekend(day) && !isDayCompleted(day) && selectDate(day)"
        >
            {{ day }}
        </div>
    </div>

    <!-- Fecha seleccionada -->
    <div v-if="selectedDate" class="mt-3 alert alert-info">
      <strong>Seleccionado:</strong> {{ selectedDate.toDateString() }}
    </div>
  </div>
</template>

<style>
    .completed{
        background-color: #e87c86 !important;
        color: white !important;
        pointer-events: none;
        opacity: 0.65;
    }
    .weekend{
        color: #fff;
        background-color: #6c757d;
        border-color: #6c757d;
        pointer-events: none;
        opacity: 0.65;
    }
</style>
