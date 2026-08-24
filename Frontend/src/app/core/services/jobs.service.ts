import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../config/api.config';
import { RunEscalationCheckResultDto } from '../models/job.model';

@Injectable({ providedIn: 'root' })
export class JobsService {
  private readonly baseUrl = API_CONFIG.JOBS;

  constructor(private readonly http: HttpClient) {}

  runEscalationCheck(): Observable<RunEscalationCheckResultDto> {
    return this.http.post<RunEscalationCheckResultDto>(`${this.baseUrl}/escalation-check`, {});
  }
}
