<script setup>
import Calendar from './components/Calendar.vue'
import Schedule from './components/Schedule.vue'
import { ref } from 'vue'

const selectedHours = ref([])
const selectedDate = ref('')
const appointmentHour = ref(-1) // ✅ faltaba
const firstHour = ref(-1)       // ✅ faltaba
const email = ref('')
const notification = ref({ type: '', message: '' })
const calendarRef = ref(null)

const updateSelectedHours = ({ date, hours }) => {
  selectedDate.value = date
  selectedHours.value = hours

  // Buscar primera hora disponible
  let firstAvailable = -1
  for (let i = 9; i <= 18; i++) {
    if (!hours.includes(i)) {
      firstAvailable = i
      break
    }
  }
  appointmentHour.value = firstAvailable
  firstHour.value = firstAvailable
}

const validarYEnviar = () => {
  if (selectedDate.value === '') {
    showNotification('danger', 'Selecciona una fecha.')
    return
  }

  if (appointmentHour.value === -1) {
    showNotification('danger', 'Selecciona una hora disponible.')
    return
  }

  const correo = email.value.trim()
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!emailRegex.test(correo)) {
    showNotification('danger', 'Ingresa un correo válido.')
    return
  }

  const cita = {
    date: selectedDate.value,
    startTime: `${appointmentHour.value}:00`,
    contactEmail: correo
  }

  console.log('Enviando cita:', cita)

  fetch('http://localhost:5077/api/appointment', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(cita)
  })
    .then(async response => {
      if (response.status === 201) {
        showNotification('success', 'Cita agendada exitosamente ☠️')
        onHourTaken(appointmentHour.value)
        const data = await response.json()
        console.log('Respuesta del servidor:', data)

        if (calendarRef.value) {
          calendarRef.value.fetchTakenAppointments();
          updateSelectedHours({
            date: selectedDate.value,
            hours: selectedHours.value
          })
        }
      } else if (response.status === 400) {
        const contentType = response.headers.get('Content-Type')
        if (contentType && contentType.includes('application/json')) {
          const errorData = await response.json()
          const errorMessages = Object.values(errorData)
            .flat()
            .join(' | ')
          showNotification('danger', `Error de validación: ${errorMessages}`)
        } else {
          const errorText = await response.text()
          showNotification('danger', errorText)
        }
      } else if (response.status === 409) {
        const errorText = await response.text()
        showNotification('danger', errorText)
      } else {
        showNotification('danger', 'Error inesperado al agendar la cita.')
      }
    })
    .catch(error => {
      console.error(error)
      showNotification('danger', 'No se pudo conectar con el servidor.')
    })
}

const showNotification = (type, message) => {
  notification.value = { type, message }
  setTimeout(() => {
    notification.value = { type: '', message: '' }
  }, 4000)
}

const onHourTaken = (hora) => {
  if (!selectedHours.value.includes(hora)) {
    // selectedHours.value.push(hora)
    selectedHours.value = [...selectedHours.value, hora]

  }
}
</script>


<template>
    <!-- Notificación -->
    <div
      v-if="notification.message"
      :class="`alert alert-${notification.type} alert-dismissible fade show`"
      role="alert"
      style="position: fixed; top: 10px; left: 50%; transform: translateX(-50%); width: 500px; z-index: 1000;"
    >
      {{ notification.message }}
      <button
        type="button"
        class="btn-close"
        @click="notification.message = ''"
        aria-label="Close"
      ></button>
    </div>

  <header style="display: flex; flex-direction: column; align-items: center;">
    <h1 class="text-center my-4">Agenda tu hora con la Muerte ☠️</h1>
    <!-- <Calendar /> -->
    <!-- <Calendar @update-hours="updateSelectedHours" /> -->
    <Calendar ref="calendarRef" @update-hours="updateSelectedHours" />


  </header>

  <main class="container">
    <p class="lead">Selecciona una hora disponible</p>
    <div class="form-group" style="margin-bottom: 10px;">
      <input v-model="email" type="email" class="form-control" id="exampleInputEmail1" aria-describedby="emailHelp" placeholder="Correo de contacto">
    </div>
    <Schedule 
      :selected-hours="selectedHours" 
      :selected-day="selectedDate"
      @update-hour="hora => appointmentHour = hora"
      @hour-taken="onHourTaken"
    />


    <button
      type="button"
      class="btn btn-info btn-lg w-100"
      style="margin-top: 20px"
      :disabled="(firstHour == -1 || selectedDate == '')"
      @click="validarYEnviar"
    >
      Agendar
    </button>
  </main>
</template>

<style>
  .hour-line{
    width: 95% !important;
  }
</style>
