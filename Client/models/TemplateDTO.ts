import { DataType } from '~/models/Enums/DataType';
import { PatchOption } from './Enums/PatchOption';

export interface TemplateGetDTO {
  name: string;
  attributes: TemplateAttributeGetDTO[];
  id: number;
}

export interface TemplateAttributeGetDTO {
  attributeId: number;
  typeId: number;
  name: string;
  type: string;
  dataType: DataType;
  featured: boolean;
  id: number;
  usesDefinedValues: boolean,
  usesDefinedUnits: boolean,
}

export interface TemplatePostDTO {
  name: string;
  attributes: TemplateAttributePostDTO[];
}

export interface TemplateAttributePostDTO {
  attributeId: number;
  featured: boolean;
}

export interface TemplatePatchDTO {
  id: number;
  name: string;
  attributes: TemplateAttributePatchDTO[];
}

export interface TemplateAttributePatchDTO {
  id: number | null;
  featured: boolean;
  attributeId: number;
  patchOption: PatchOption;
}
