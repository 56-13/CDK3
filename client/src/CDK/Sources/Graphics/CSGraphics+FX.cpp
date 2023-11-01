#define CDK_IMPL

#include "CSGraphics.h"

#include "CSShaders.h"

#include "CSRenderers.h"

static void getQuad(const CSRect& rect, const CSMatrix& wvp, CSVector2* result) {
	result[0] = (CSVector2)CSVector3::transformCoordinate(rect.leftTop(), wvp);
	result[1] = (CSVector2)CSVector3::transformCoordinate(rect.rightTop(), wvp);
	result[2] = (CSVector2)CSVector3::transformCoordinate(rect.leftBottom(), wvp);
	result[3] = (CSVector2)CSVector3::transformCoordinate(rect.rightBottom(), wvp);
}

void CSGraphics::blur(float intensity) {
	CSRenderTarget* target = _state->batch.target;

	CSDelegateRenderCommand* command = this->command([target, intensity](CSGraphicsApi* api) {
		target->focus(api);

		CSShaders::blur()->draw(api, intensity);
	});

	command->addFence(target, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);
}

void CSGraphics::blur(const CSRect& rect, float intensity) {
	CSRenderTarget* target = _state->batch.target;
	CSMatrix wvp = worldViewProjection();

	CSDelegateRenderCommand* command = this->command([target, wvp, rect = CSRect(rect), intensity](CSGraphicsApi* api) {
		target->focus(api);

		CSVector2 quad[4];
		getQuad(rect, wvp, quad);
		CSShaders::blur()->draw(api, quad, intensity);
	});
	
	command->addFence(target, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);
}

void CSGraphics::blurDepth(float distance, float range, float intensity) {
	CSRenderTarget* target = _state->batch.target;
	CSCamera camera = _state->batch.camera;

	CSDelegateRenderCommand* command = this->command([target, camera, distance, range, intensity](CSGraphicsApi* api) {
		target->focus(api);

		CSShaders::blur()->drawDepth(api, camera, distance, range, intensity);
	});

	command->addFence(target, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);
}

void CSGraphics::blurDepth(const CSRect& rect, float distance, float range, float intensity) {
	CSRenderTarget* target = _state->batch.target;
	CSMatrix wvp = worldViewProjection();
	CSCamera camera = _state->batch.camera;

	CSDelegateRenderCommand* command = this->command([target, wvp, rect = CSRect(rect), camera, distance, range, intensity](CSGraphicsApi* api) {
		target->focus(api);

		CSVector2 quad[4];
		getQuad(rect, wvp, quad);
		CSShaders::blur()->drawDepth(api, quad, camera, distance, range, intensity);
	});

	command->addFence(target, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);
}

void CSGraphics::blurDirection(const CSVector2& dir) {
	CSRenderTarget* target = _state->batch.target;

	CSDelegateRenderCommand* command = this->command([target, dir = CSVector2(dir)](CSGraphicsApi* api) {
		target->focus(api);

		CSShaders::blur()->drawDirection(api, 0, dir);
	});

	command->addFence(target, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);
}

void CSGraphics::blurDirection(const CSRect& rect, const CSVector2& dir) {
	CSRenderTarget* target = _state->batch.target;
	CSMatrix wvp = worldViewProjection();

	CSDelegateRenderCommand* command = this->command([target, wvp, rect = CSRect(rect), dir = CSVector2(dir)](CSGraphicsApi* api) {
		target->focus(api);

		CSVector2 quad[4];
		getQuad(rect, wvp, quad);
		CSShaders::blur()->drawDirection(api, quad, dir);
	});

	command->addFence(target, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);
}

void CSGraphics::blurCenter(const CSVector2& center, float range) {
	CSRenderTarget* target = _state->batch.target;

	CSDelegateRenderCommand* command = this->command([target, center = CSVector2(center), range](CSGraphicsApi* api) {
		target->focus(api);

		CSShaders::blur()->drawCenter(api, 0, center, range);
	});

	command->addFence(target, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);
}

void CSGraphics::blurCenter(const CSRect& rect, const CSVector2& center, float range) {
	CSRenderTarget* target = _state->batch.target;
	CSMatrix wvp = worldViewProjection();

	CSDelegateRenderCommand* command = this->command([target, wvp, rect = CSRect(rect), center = CSVector2(center), range](CSGraphicsApi* api) {
		target->focus(api);

		CSVector2 quad[4];
		getQuad(rect, wvp, quad);
		CSShaders::blur()->drawCenter(api, quad, center, range);
	});

	command->addFence(target, CSGBatchFlagReadWrite | CSGBatchFlagRetrieve);
}

void CSGraphics::lens(const CSVector3& center, float radius, float convex) {
	if (_state->batch.renderer == CSRenderers::distortion()) {
		const CSTexture* screenTexture = static_assert_cast<const CSTexture*>(rendererParam());
		CSRenderTarget* target = _state->batch.target;
		CSMatrix wvp = worldViewProjection();

		CSDelegateRenderCommand* command = this->command([target, screenTexture, wvp, center = CSVector3(center), radius, convex](CSGraphicsApi* api) {
			target->focus(api);

			CSShaders::lens()->draw(api, screenTexture, wvp, center, radius, convex);
		});
		
		command->addFence(target, CSGBatchFlagWrite | CSGBatchFlagRetrieve);
	}
}

void CSGraphics::wave(const CSVector3& center, float radius, float thickness) {
	if (_state->batch.renderer == CSRenderers::distortion()) {
		const CSTexture* screenTexture = static_assert_cast<const CSTexture*>(rendererParam());
		if (screenTexture) {
			CSRenderTarget* target = _state->batch.target;
			CSMatrix wvp = worldViewProjection();

			CSDelegateRenderCommand* command = this->command([target, screenTexture, wvp, center = CSVector3(center), radius, thickness](CSGraphicsApi* api) {
				target->focus(api);

				CSShaders::wave()->draw(api, screenTexture, wvp, center, radius, thickness);
			});

			command->addFence(target, CSGBatchFlagWrite | CSGBatchFlagRetrieve);
		}
	}
}
