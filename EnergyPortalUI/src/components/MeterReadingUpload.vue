<template>
  <div class="field">
    <div class="file is-boxed is-primary has-name centered">
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
    <div class="field mt-4 centered">
      <button class="button is-primary" @click="submitFile" :disabled="isLoading">
        <span v-if="!isLoading">Submit</span>
        <span v-else class="icon is-small">
          <i class="fas fa-spinner fa-spin"></i>
        </span>
      </button>
    </div>
    <div v-if="successMessage" class="notification is-success mt-4 centered">
      <div class="success-details">
        <div>{{ successMessage }}</div>
        <div>Successfully processed readings: {{ successCount }}</div>
        <div>Failed readings: {{ failedCount }} <i class="fas fa-info-circle" @click="showFailedDetails"></i></div>
      </div>
    </div>
    <div v-if="errorMessage" class="notification is-danger mt-4 centered">
      <div class="fail-details">
        <div>{{ errorMessage }}</div>
      </div>
    </div>
    <div v-if="showFailedDetailsBanner" class="notification is-info mt-4">
      <p class="title is-5">Failed Meter Reading Details</p>
      <div>Parsed reading failures: {{ parsedFailures }}</div>
      <div>Validation failures: {{ validationFailures }}</div>
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
const errorMessage = ref(''); // Add this line

const showFailedDetailsBanner = ref(false);

const fileName = computed(() => file.value?.name);

const uploadFile = (event) => {
  file.value = event.target.files[0];
  resetMessages();
};

const resetMessages = () => {
  successMessage.value = '';
  successCount.value = 0;
  failedCount.value = 0;
  parsedFailures.value = 0;
  validationFailures.value = 0;
  showFailedDetailsBanner.value = false;
  errorMessage.value = ''; // Add this line
};

const submitFile = async () => {
  if (!file.value) return;

  const formData = new FormData();
  formData.append('meterReadingFile', file.value);

  isLoading.value = true;
  resetMessages();

  try {
    const response = await axios.post(`${process.env.VUE_APP_BASEURL}/api/meter-reading-uploads`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    successMessage.value = 'Meter reading file processed successfully';
    successCount.value = response.data.successfulReadingsCount;
    failedCount.value = response.data.failedReadingsCount;
    parsedFailures.value = response.data.csvParseFailures.length;
    validationFailures.value = response.data.validationFailures.length;
  } catch (error) {
    console.error('Error:', error);
    errorMessage.value = error.response?.data ?? error.message;
  } finally {
    isLoading.value = false;
  }
};

const showFailedDetails = () => {
  showFailedDetailsBanner.value = true;
};
</script>

<style scoped>
.centered {
  display: flex;
  justify-content: center;
}

.file.has-name .file-label .file-name {
  display: inline-block;
  margin-left: 1rem;
  font-weight: bold;
}

.mt-4 {
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

.success-details > div, .fail-details > div {
  margin-bottom: 0.5rem;
}
</style>
