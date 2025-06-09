<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  selectedHours: {
    type: Array,
    default: () => []
  },
  selectedDay: {
    type: String,
    default: () => ''
  }
})

const emit = defineEmits(['update-hour', 'hour-taken'])

const officeHours = Array.from({ length: 10 }, (_, i) => 9 + i)
const appointmentHour = ref(-1)
const hourHeight = 50

// Actualizar appointmentHour cada vez que cambian los selectedHours
// watch(() => props.selectedHours, (newVal) => {
//   let firstAvailable = -1
//   if(props.selectedDay != ''){
//       for (let i = 9; i <= 18; i++) {
//         if (!newVal.includes(i)) {
//           firstAvailable = i
//           break
//         }
//       }
//   }
//   appointmentHour.value = firstAvailable
// }, { immediate: true })

// Emitir cambio cada vez que se modifica
watch(appointmentHour, (newVal) => {
  emit('update-hour', newVal)
})

watch([() => props.selectedHours, () => props.selectedDay], ([newHours, newDay]) => {
  let firstAvailable = -1
  if (newDay != '') {
    for (let i = 9; i <= 18; i++) {
      if (!newHours.includes(i)) {
        firstAvailable = i
        break
      }
    }
  }
  appointmentHour.value = firstAvailable
}, { immediate: true })

const onDragStart = (e) => {
  e.dataTransfer.setData('text/plain', appointmentHour.value)
}

const onDragEnd = (e) => {
  const containerTop = e.target.parentElement.getBoundingClientRect().top
  const newTop = e.clientY - containerTop
  const newHour = 9 + Math.floor(newTop / hourHeight)

  if (newHour >= 9 && newHour <= 18 && !props.selectedHours.includes(newHour)) {
    appointmentHour.value = newHour
    // emit('hour-taken', newHour)
  }
}
</script>



<template>
    <div class="card p-3 position-relative" style="height: 500px;">
        <!-- Líneas de hora -->
        <div
            v-for="oh in officeHours"
            :key="oh"
            :style="{ top: `${(oh - 9) * hourHeight}px` }"
            class="d-flex align-items-center position-absolute hour-line"
        >
            <span class="fw-lighter text-muted" style="font-size: 12px; width: 45px;">
            {{ oh < 10 ? `0${oh}` : oh }}:00
            </span>
            <hr style="flex: 1; margin: 0;">
        </div>

        <!-- Citas bloqueadas -->
        <button
            v-for="h in selectedHours"
            :key="'blocked-' + h"
            class="btn btn-danger text-white hour"
            disabled
            style="position: absolute; left: 60px; width: calc(100% - 70px); height: 45px;"
            :style="{ top: `${(h - 9) * hourHeight}px` }"
        >
            Reservado
        </button>

        <!-- Cita arrastrable -->
        <div
        class="btn btn-primary text-white hour"
        style="position: absolute; left: 60px; width: calc(100% - 70px); height: 45px;"
        :style="{ top: `${(appointmentHour - 9) * hourHeight}px`, display: (appointmentHour == -1) ? 'none' : 'unset' }"
        draggable="true"
        @dragstart="onDragStart"
        @dragend="onDragEnd"
        >
        Reservar cita ☠️
        </div>


    </div>
</template>

<style>
</style>
