<script setup lang="ts">
import { reactive, ref, computed } from 'vue'
import LinkService from '@/services/LinkService'
import useClipboard from 'vue-clipboard3'

const { toClipboard } = useClipboard()

enum TimeInterval {
  Minutes = 'minute(s)',
  Hours = 'hour(s)',
  Days = 'day(s)',
  Weeks = 'week(s)'
}

const intervals: TimeInterval[] = [
    TimeInterval.Minutes,
    TimeInterval.Hours,
    TimeInterval.Days,
    TimeInterval.Weeks
]

interface Form {
  url: string
  timeValue: number
  timeInterval: TimeInterval
  neverExpires: boolean
}

const formData = reactive<Form>({
  url: '',
  timeValue: 0,
  timeInterval: TimeInterval.Minutes,
  neverExpires: false
})

const shouldShowSnackbar = ref<boolean>(false)
const link = ref<string>()
const fullUrl = computed(() => `${window.location.origin}/to/${link.value}`)

async function submit() {
  if (formData.neverExpires) {
    link.value = (await LinkService.createLink(formData.url)) ?? ''
  } else {
    let duration: string = ''
    switch (formData.timeInterval) {
      case TimeInterval.Minutes:
        duration = `0.00:${formData.timeValue}:00.000`
        break
      case TimeInterval.Hours:
        duration = `0.${formData.timeValue}:00:00.000`
        break
      case TimeInterval.Days:
        duration = `${formData.timeValue}.00:00:00.000`
        break
      case TimeInterval.Weeks:
        duration = `${formData.timeValue * 7}.00:00:00.000`
        break
    }
    link.value = (await LinkService.createLink(formData.url, duration)) ?? ''
  }
  if (link.value) shouldShowSnackbar.value = true
}
</script>

<template>
  <v-snackbar v-model="shouldShowSnackbar">
    Link created at <a v-if="link" :href="`/to/${link}`">{{ fullUrl }}!</a>

    <template v-slot:actions>
      <v-btn icon="mdi-content-copy" @click="async () => await toClipboard(fullUrl)" />
      <v-btn icon="mdi-close-box" @click="() => (shouldShowSnackbar = false)" />
    </template>
  </v-snackbar>
  <v-form>
    <v-container>
      <v-row justify="center">
        <v-col cols="8">
          <v-text-field
            hide-details="auto"
            label="Url to Shorten"
            v-model="formData.url"
            variant="solo"
          />
        </v-col>
      </v-row>
      <v-row justify="center">
        <v-col cols="3">
          <v-text-field
            type="number"
            hide-details="auto"
            class="mx-2"
            variant="solo"
            :disabled="formData.neverExpires"
            v-model="formData.timeValue"
          />
        </v-col>
        <v-col cols="3">
          <v-select
            hide-details="auto"
            class="mx-2"
            variant="solo"
            :disabled="formData.neverExpires"
            :items="intervals"
            v-model="formData.timeInterval"
            label="Time Increment"
          />
        </v-col>
        <v-col cols="2">
          <v-checkbox
            hide-details="auto"
            class="mx-2"
            v-model="formData.neverExpires"
            label="Never Expires"
          />
        </v-col>
      </v-row>
      <v-row justify="center" class="pa-2">
        <v-btn color="primary" @click="submit"> Shorten Url </v-btn>
      </v-row>
    </v-container>
  </v-form>
</template>

<style scoped></style>
