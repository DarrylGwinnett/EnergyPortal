<template>
  <div class="field">
    <div class="file is-boxed is-primary has-name" style="display: flex; justify-content: center;">
      <label class="file-label">
        <input class="file-input" type="file" @change="uploadFile">
        <span class="file-cta">
          <span class="file-icon">
            <i class="fas fa-upload"></i>
          </span>
          <span class="file-label">
            Choose a file…
          </span>
        </span>
        <span class="file-name" v-if="fileName">
          {{ fileName }}
        </span>
      </label>
    </div>
    <div class="field mt-4" style="display: flex; justify-content: center;">
      <button class="button is-primary" @click="submitFile" :disabled="isLoading">
        <span v-if="!isLoading">Submit</span>
        <span v-else class="icon is-small">
          <i class="fas fa-spinner fa-spin"></i>
        </span>
      </button>
    </div>
    <div v-if="successMessage" class="notification is-success mt-4">
      <div>{{ successMessage }}</div>
      <div>Successfully processed readings: {{ successCount }}</div>
      <div>Failed readings: {{ failedCount }} <span class="details-link" @click="showFailedDetails">Click for details</span></div>
    </div>
    <div v-if="showFailedDetailsBanner" class="notification is-info mt-4">
      <p class="title is-5">Failed Meter Reading Details</p>
      <div>Parsed reading failues: {{ parsedFailures }}</div>
      <div>Validation Failures: {{ validationFailures }} </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import axios from 'axios';

const file = ref(null);
const isLoading = ref(false);
const successMessage = ref('');
const successCount = ref(0);
const failedCount = ref(0);
const parsedFailures = ref(0);
const validationFailures = ref(0);

const showFailedDetailsBanner = ref(false);

const fileName = computed(() => file.value?.name);

const uploadFile = (event) => {
  file.value = event.target.files[0];
  successMessage.value = '';
  successCount.value = 0;
  failedCount.value = 0;
  showFailedDetailsBanner.value = false;
  console.log('File selected:', file.value);
};

const submitFile = async () => {
  console.log('Submitting file:', file);
  if (!file.value) return;

  const formData = new FormData();
  formData.append('meterReadingFile', file.value);

  isLoading.value = true;
  successMessage.value = '';
  successCount.value = 0;
  failedCount.value = 0;

  try {
    let response = await axios.post(process.env.VUE_APP_BASEURL + '/api/meter-reading-uploads', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    console.log('Response:', response);
    successMessage.value = 'Meter reading file processed successfully';
    successCount.value = response.data.successfulReadingsCount;
    failedCount.value = response.data.failedReadingsCount;
    parsedFailures.value = response.data.csvParseFailures.length;
    validationFailures.value = response.data.validationFailures.length;
  } catch (error) {
    console.error('Error:', error);
  } finally {
    isLoading.value = false;
  }
};

const showFailedDetails = () => {
  showFailedDetailsBanner.value = true;
};
</script>

<style scoped>
.file.has-name .file-label .file-name {
  display: inline-block;
  margin-left: 1rem;
  font-weight: bold;
}

.field.mt-4 {
  margin-top: 1rem;
}

.button.is-primary {
  background-color: #1af387de;
  border-color: transparent;
  color: #fff;
}

.button.is-primary:hover {
  background-color: #00b89c;
  border-color: transparent;
  color: #fff;
}

.icon.is-small {
  font-size: 1em;
}

.fa-spinner.fa-spin {
  animation: fa-spin 2s infinite linear;
}

@keyframes fa-spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.notification.mt-4 {
  margin-top: 1rem;
}

.details-link {
  cursor: pointer;
  text-decoration: underline;
  color: blue;
}

.details-link:hover {
  color: darkblue;
}
</style>
